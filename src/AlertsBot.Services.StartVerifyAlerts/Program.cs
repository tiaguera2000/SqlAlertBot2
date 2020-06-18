using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AlertsBot.Infra.CrossCutting.IoC;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MediatR;

namespace AlertsBot.Services.StartVerifyAlerts
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            System.Console.WriteLine("Entrou na Program");
            var builder = new HostBuilder();

            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                config.AddEnvironmentVariables();

                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            });

            builder.ConfigureServices((hostingContext, services) =>
            {
                services.RegisterServices(hostingContext.Configuration);
                services.AddOptions();
                services.AddMediatR(typeof(Program));
                services.AddSingleton<IHostedService, Alertas>();
            });

            builder.ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddLog4Net(hostingContext.Configuration.GetValue<string>("Log4NetConfigFile"));
            });


            await builder.RunConsoleAsync();
        }
    }
}
