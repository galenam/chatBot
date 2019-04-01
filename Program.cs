using System;
using System.Collections.Specialized;
using System.IO;
using Microsoft.Extensions.Configuration;
using Model;
using Quartz;
using Quartz.Impl;
using Microsoft.Extensions.Configuration.FileExtensions;
using System.Threading.Tasks;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using BotConsole.Const;

namespace BotConsole
{
    class Program
    {
        static IScheduler Scheduler;
        static ApplicationModel appModel;
        static TelegramBotClient _botClient;

        public static async Task Main(string[] args)
        {
            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            var factory = new StdSchedulerFactory(props);

            Scheduler = await factory.GetScheduler();
            await Scheduler.Start();
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            appModel = configuration.GetSection("ApplicationModel").Get<ApplicationModel>();

            NetworkCredential credentials = null;
            var userName = appModel.ProxyConfiguration.UserName;
            var password = appModel.ProxyConfiguration.Password;
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                credentials = new NetworkCredential(userName, password);
            var httpProxy = new WebProxy(appModel.ProxyConfiguration.Url, appModel.ProxyConfiguration.Port)
            {
                Credentials = credentials
            };

            _botClient = new TelegramBotClient(appModel.BotConfiguration.BotToken, httpProxy);

            _botClient.OnMessage += Bot_OnMessage;
            _botClient.OnReceiveError += BotOnReceiveError;

            _botClient.StartReceiving(Array.Empty<UpdateType>());
            string name = string.Empty;
            try
            {
                var tmp = _botClient.GetMeAsync();
                name = tmp.Result.Username;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine($"Start listening for @{name}");
            Console.ReadLine();
            _botClient.StopReceiving();
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            Console.WriteLine(e);
        }

        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                var me = await _botClient.GetMeAsync();
                var text = e.Message.Text;
                var resultText = string.Empty;
                if (string.IsNullOrEmpty(text))
                {
                    resultText = "Please, choose the command";
                    return;
                }
                switch (text)
                {
                    case var included when text.ToLower().Contains(CommandConst.Reminder):
                        // todo: добавить напоминания на всех будущие (???) дз 
                        break;
                }

                //System.Console.WriteLine($"Hello! My name is {me.FirstName}");
                //             Message message = await _botClient.SendTextMessageAsync(
                //   chatId: e.Message.Chat.Id, // or a chat id: 123456789
                //   text: e.Message.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + "Error message sending");
            }
        }
    }
}
