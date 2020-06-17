using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SqlAlertBot2.Services;
using System;
using Telegram.Bot;

namespace SqlAlertBot2
{
    class Program
    {
        public static readonly TelegramBotClient bot = new TelegramBotClient("1234289330:AAHE5ZCIfVXXhaS2dwzNiFcwnaWyH6FgJys");

        public static async System.Threading.Tasks.Task Main(string[] args)
        {

            string message = Verifier.Verify();
            await Sender.Send(message, bot);


        }

    }
}
