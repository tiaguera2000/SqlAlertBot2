using System;
using Telegram.Bot;

namespace SqlAlertBot2
{
    class Program
    {
        private static readonly TelegramBotClient bot = new TelegramBotClient("1234289330:AAHE5ZCIfVXXhaS2dwzNiFcwnaWyH6FgJys");

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            /*bot.OnMessage += Bot_OnMessage;
            bot.StartReceiving();
            Console.ReadLine();
            bot.StopReceiving();*/

            await bot.SendTextMessageAsync("-444824024", "olá!");
        }
        /*private static async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
        }*/
    }
}
