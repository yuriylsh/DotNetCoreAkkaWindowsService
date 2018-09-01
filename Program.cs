using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetCoreAkkaWindowsService
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var currentDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            Directory.SetCurrentDirectory(currentDirectory);

            var builder = new HostBuilder()
                .ConfigureAppConfiguration(appConfig => appConfig.AddJsonFile("appsettings.json"))
                .ConfigureServices(services => services.AddHostedService<AkkaBackgroundService>());
            var isDebugging = IsDebugging(args);
            builder.ConfigureSerilogLogging(isDebugging);

            if (isDebugging)
            {
                await builder.RunConsoleAsync();
            }
            else
            {
                await builder.RunAsServiceAsync();
            }
        }

        private static bool IsDebugging(ICollection<string> args)
        {
#if DEBUG
            return true;
#else
            return args.Contains("--console");
#endif
        }
    }
}