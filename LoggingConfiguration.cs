using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace DotNetCoreAkkaWindowsService
{
    public static class LoggingConfiguration
    {
        public delegate void StopAndFlushLogging();

        public static void ConfigureSerilogLogging(this IHostBuilder hostBuilder, bool isDebugging)
        {
            ConfigureSerilog(isDebugging);
            hostBuilder
                .ConfigureLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
                .ConfigureServices(services => services.AddSingleton<StopAndFlushLogging>(() =>
                {
                    Log.Logger.Information("Application is exiting. Flushing and stopping logging...");
                    Log.CloseAndFlush();
                }));
        }

        private static void ConfigureSerilog(bool isDebugging)
        {
            var configuration = new LoggerConfiguration()
                .WriteTo.Async(a => a.File("log.txt",
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 104_857_600, // 100MB
                    retainedFileCountLimit: 3));
            configuration = isDebugging
                ? configuration.MinimumLevel.Debug().WriteTo.Console()
                : configuration.MinimumLevel.Information().MinimumLevel
                    .Override("Microsoft", LogEventLevel.Warning);

            Log.Logger = configuration.CreateLogger();
        }
    }
}