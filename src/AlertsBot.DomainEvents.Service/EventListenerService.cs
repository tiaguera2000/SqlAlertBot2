using log4net;
using Microsoft.Extensions.Hosting;
using SharedKernel.DomainEvents.CrossDomainEvents.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using AlertsBot.DomainEvents.Service.Handlers;
using AlertsBot.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AlertsBot.DomainEvents.Service
{
    public class EventListenerService : IHostedService, IDisposable
    {
        private readonly ILog _logger;
        private readonly IListener _listener;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITelegramAppService _serviceTelegram;
        private readonly IConfiguration _configuration;

        public EventListenerService(ILog logger, IListener listener, IServiceProvider serviceProvider, ITelegramAppService telegramAppService, IConfiguration configuration)
        {
            _logger = logger;
            _listener = listener;
            _serviceProvider = serviceProvider;
            _serviceTelegram = telegramAppService;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("AlertsBot.DomainEvents.Service starting...");
            _listener.AddHandler(new OnSendMessageTelegramGroup(_logger, _serviceProvider, _serviceTelegram, _configuration));
            _logger.Info("AlertsBot.DomainEvents.Service starded...");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info("AlertsBot.DomainEvents.Service stopping...");

            _listener.CloseConnections();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}