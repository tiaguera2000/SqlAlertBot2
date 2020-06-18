using AlertsBot.Application.Interfaces;
using AlertsBot.Infra.CrossCutting.IoC;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Telegram.Bot;

namespace AlertsBot.Services.Telegram
{
    class Program
    {
        static void Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
           

            Console.WriteLine($"environmentName = {environmentName}");
            Console.WriteLine($"CurrentDirectory = {Directory.GetCurrentDirectory()}");

            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{environmentName}.json", optional: true);

            builder.AddEnvironmentVariables();

            var configuration = builder.Build();

            var services = new ServiceCollection();

            // .NET Native DI Abstraction
            BootStrapper.RegisterServices(services, configuration);
            services.AddScoped<AlertsBot>();

            var serviceProvider = services.BuildServiceProvider();

            var log = serviceProvider.GetService<ILog>();


            try
            {
                var appService = serviceProvider.GetService<AlertsBot>();
                Console.ReadKey();

            }
            catch (Exception ex)
            {
                log.Error(ex);
                Environment.Exit(1);
            }
            finally
            {
                 Environment.Exit(0);
            }
        }
    }
  
}
