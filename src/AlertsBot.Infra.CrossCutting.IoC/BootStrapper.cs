using AutoMapper;
using AlertsBot.Application.Interfaces;
using AlertsBot.Application.Services;
using AlertsBot.Domain.Core.Bus;
using AlertsBot.Domain.Core.Notifications;
using AlertsBot.Domain.Interfaces;
using AlertsBot.Infra.CrossCutting.Bus;
using AlertsBot.Infra.Data.Repository;
using AlertsBot.Infra.SqlServer.UoW;
using log4net;
using log4net.Config;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SharedKernel.Logger;
using SharedKernel.Logger.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using SharedKernel.DomainEvents.Core;
using SharedKernel.DomainEvents.CrossDomainEvents;
using SharedKernel.DomainEvents.CrossDomainEvents.Configuration;
using SharedKernel.DomainEvents.CrossDomainEvents.Interfaces;
using AlertsBot.Domain.Code;

namespace AlertsBot.Infra.CrossCutting.IoC
{
    public static class BootStrapper
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {

            // Configuration
            services.AddSingleton<IConfiguration>(configuration);

            // Getting all assemblies to run
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Union(new Assembly[]
                {
                }).ToArray();

            // Automapper
            services.AddAutoMapper(assemblies);

            // Adding MediatR for Domain Events and Notifications
            services.AddMediatR(assemblies);

            // App Service
            services.AddScoped<ITelegramAppService, TelegramAppService>();

            // Domain Bus (Mediator)
            services.AddScoped<IMediatorHandler, InMemoryBus>();

            // Domain - Events
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            // Infra - Data

            // Infra - Register Repositories
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ICurriculum, CurriculumRepository>();
            services.AddScoped<IPayment, PaymentRepository>();

            // log4net
            XmlConfigurator.ConfigureAndWatch(LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly()), new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configuration["Log4NetConfigFile"])));
            services.AddSingleton<ILoggerRepository, LogRepository>();
            services.AddSingleton<ILog>(sp =>
            {
                return sp.GetService<ILoggerRepository>().GetLogger("TermSearch");
            });

            // Infra - Data
            services.AddScoped<IDbConnection>(sp =>
            {
                return sp.GetService<Func<string, IDbConnection>>()("ConnectionStrings");
            });

            services.AddScoped<IUnitOfWork>(sp =>
            {
                return sp.GetService<Func<string, IUnitOfWork>>()("ConnectionStrings");
            });

            services.AddScoped<Dictionary<string, List<IDbConnection>>>();
            services.AddScoped<Dictionary<string, IUnitOfWork>>();

            services.AddScoped<Func<string, List<IDbConnection>>>(sp => key =>
            {
                var dbConnections = sp.GetService<Dictionary<string, List<IDbConnection>>>();

                if (!dbConnections.ContainsKey(key))
                {
                    if (string.IsNullOrEmpty(key))
                        key = "DefaultConnection";


                    var connectionString = configuration.GetSection(key);
                    List<IDbConnection> dbConnection = new List<IDbConnection>() {

                        new SqlConnection(connectionString["DefaultConnection"]),
                        new SqlConnection(connectionString["SinePRD"]),
                    };
                    //if (string.IsNullOrEmpty(connectionString))
                    //    throw new KeyNotFoundException($"Connection string '{key}'is not present on the configuration.");

                    dbConnections.Add(key, dbConnection);
                }

                return dbConnections[key];
            });

            services.AddScoped<Func<string, IUnitOfWork>>(sp => key =>
            {
                var units = sp.GetService<Dictionary<string, IUnitOfWork>>();

                if (!units.ContainsKey(key))
                {
                    var connProvider = sp.GetService<Func<string, List<IDbConnection>>>();
                    var log = sp.GetService<ILog>();

                    units.Add(key, new UnitOfWork(connProvider(key), new NpgsqlConnection(configuration.GetConnectionString("PostgresSql")), log));
                }

                return units[key];
            });

            // Crossdomain
            var crossdomainEventsSettings = configuration.GetSection("CrossDomainEventsSettings");
            services.AddSingleton(new CrossDomainEventsSettings
            {
                Host = crossdomainEventsSettings["Host"],
                User = crossdomainEventsSettings["User"],
                Password = crossdomainEventsSettings["Password"],
                VirtualHost = crossdomainEventsSettings["VirtualHost"],
                AutomaticRecoveryEnabled = Convert.ToBoolean(crossdomainEventsSettings["AutomaticRecoveryEnabled"]),
                ContinuationTimeout = TimeSpan.FromSeconds(Convert.ToInt32(crossdomainEventsSettings["ContinuationTimeout"])),
                NetworkRecoveryInterval = TimeSpan.FromSeconds(Convert.ToInt32(crossdomainEventsSettings["NetworkRecoveryInterval"])),
                RequestedHeartbeat = Convert.ToUInt16(crossdomainEventsSettings["RequestedHeartbeat"]),
                TopologyRecoveryEnabled = Convert.ToBoolean(crossdomainEventsSettings["TopologyRecoveryEnabled"])
            });

            services.AddSingleton<NamesResolver>();
            services.AddSingleton<INamesResolver>(x => x.GetService<NamesResolver>());
            services.AddSingleton<IHandler<ICrossDomainEvent>, CrossDomainEventHandler>();
            services.AddSingleton<IHandlerNamesResolver<ICrossDomainEvent>, CrossDomainEventHandler>();
            services.AddSingleton<IChannelsManager, ChannelsManager>();
            services.AddScoped<IListener, Listener>();

        }
    }
}
