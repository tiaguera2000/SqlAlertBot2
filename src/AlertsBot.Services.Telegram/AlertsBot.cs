using AlertsBot.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace AlertsBot.Services.Telegram
{
    public class AlertsBot : IDisposable
    {
        public TelegramBotClient telegram;
        private readonly ITelegramAppService servcice;

        public AlertsBot(IConfiguration configuration, ITelegramAppService servcice)
        {
            this.telegram = new TelegramBotClient(configuration["TelegramKey"]);
            //this.telegram.OnMessage += Bot_onMessage;
            this.telegram.StartReceiving();
            this.servcice = servcice;
        }
        //Comentando para que o bot atue apenas enviando, não recebe comando!
        //public void Bot_onMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        //{

        //    if (e.Message.Text == null)
        //    {
        //        telegram.SendTextMessageAsync(e.Message.Chat.Id, "");
        //    }
        //    else
        //    {
        //        telegram.SendTextMessageAsync(e.Message.Chat.Id, servcice.ExecuteCommandAsync(e.Message.Text).Result, replyToMessageId: e.Message.MessageId);
        //    }
        //}

        public async Task<bool> SendTextMessageAsync(string message, long chatId)
        {
            try
            {
                
                await telegram.SendTextMessageAsync(chatId, message, disableNotification:true);
                return true;
            }
            catch(ApiRequestException ex)
            {
                throw;
            }
        }

        public void Dispose()
        {
            this.telegram.StopReceiving();
        }
    }
}
