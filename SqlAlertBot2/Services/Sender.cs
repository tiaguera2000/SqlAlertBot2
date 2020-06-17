using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace SqlAlertBot2.Services
{
    
    public static  class Sender
    {
        public static async Task Send(string message, TelegramBotClient bot)
        {
            string GroupId = "-444824024";

            await bot.SendTextMessageAsync(GroupId, message);
        }
    }
}
