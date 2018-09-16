using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using DotNetCoreAkkaWindowsService.Actors;
using LanguageExt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetCoreAkkaWindowsService
{
    public class AkkaBackgroundService : BackgroundService
    {
        private readonly ILogger<AkkaBackgroundService> _logger;
        private readonly IConfiguration _configuration;

        public AkkaBackgroundService(
            ILogger<AkkaBackgroundService> logger
            , IConfiguration configuration
            , IApplicationLifetime applicationLifetime
            , LoggingConfiguration.StopAndFlushLogging stopAndFlushLogging)
        {
            _logger = logger;
            _configuration = configuration;
            applicationLifetime.ApplicationStopped.Register(() => stopAndFlushLogging());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await LoadAkkaConfig().MatchAsync(
                akkaConfig => Task.Run(() =>
                {
                    Run(akkaConfig, stoppingToken);
                    return Unit.Default;
                }, stoppingToken),
                () =>
                {
                    _logger.LogInformation(nameof(AkkaBackgroundService) + " is done.");
                    return Unit.Default;
                });
        }

        private void Run(Config akkaConfig, CancellationToken stoppingToken)
        {
            var system = ActorSystem.Create(nameof(AkkaBackgroundService), akkaConfig);
            SetupAkkaDependencyInjection(system);
            var updateActor = system.ActorOf(system.DI().Props<ParentActor>(), nameof(ParentActor));
            system.Scheduler.ScheduleTellRepeatedly(TimeSpan.Zero, TimeSpan.FromSeconds(3), updateActor, 123, ActorRefs.NoSender );
            try
            {
                system.WhenTerminated.Wait(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Akka System has been terminated.");
            }
        }

        private void SetupAkkaDependencyInjection(ActorSystem actorSystem)
        {
            var autofacContainerBuilder = new ContainerBuilder();
            autofacContainerBuilder.RegisterInstance(_configuration);
            autofacContainerBuilder.RegisterInstance(new ActorInjector()).As<IActorInjector>();
            autofacContainerBuilder.RegisterType<ParentActor>();
            autofacContainerBuilder.RegisterType<ChildActor>();
            new AutoFacDependencyResolver(autofacContainerBuilder.Build(), actorSystem);
        }

        private OptionAsync<Config> LoadAkkaConfig()
        {
            TryAsync<Config> tryLoadAkkaConfig =
                async () => ConfigurationFactory.ParseString(await File.ReadAllTextAsync("hocon.txt"));

            return tryLoadAkkaConfig.Match(
                akkaConfig => akkaConfig,
                ex =>
                {
                    _logger.LogError("Failed to load akka config");
                    return (Config) null;
                });
        }
    }
}