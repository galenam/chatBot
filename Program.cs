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
            _botClient.StartReceiving();
        }

        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                var me = await _botClient.GetMeAsync();
                //System.Console.WriteLine($"Hello! My name is {me.FirstName}");
                Message message = await _botClient.SendTextMessageAsync(
      chatId: 0,//e.Message.Chat, // or a chat id: 123456789
      text: "test");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + "Error message sending");
            }
        }
    }
}
