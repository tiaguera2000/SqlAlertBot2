using AlertsBot.Domain.Events.Core;
using AlertsBot.Domain.Core.Events;
using log4net;
using System;
using SharedKernel.DomainEvents.Core;
using System.Threading.Tasks;
using AlertsBot.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using AlertsBot.Services.Telegram;
using Telegram.Bot.Exceptions;

namespace AlertsBot.DomainEvents.Service.Handlers
{
    public class OnSendMessageTelegramGroup : IHandler<SendMessageTelegramGroup>
    {
        private readonly ILog _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITelegramAppService _serviceTelegram;
        private readonly IConfiguration _configuration;
        public OnSendMessageTelegramGroup(ILog logger, IServiceProvider serviceProvider, ITelegramAppService telegramAppService, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _serviceTelegram = telegramAppService;
        }

        public void Handle(SendMessageTelegramGroup args)
        {
            var _serviceAlertsBot = new AlertsBot.Services.Telegram.AlertsBot(_configuration, _serviceTelegram);
            try
            {
                Task.Run(async () =>
                {
                    await _serviceAlertsBot.SendTextMessageAsync(args.Message, args.ChatId);
                }).GetAwaiter().GetResult();
            }
            catch (ApiRequestException ex)
            {
                throw;
            }
        }

    }
}
