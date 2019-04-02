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
using BotConsole.Code;

namespace BotConsole
{
    class Program
    {
        static IScheduler Scheduler;
        static ApplicationModel appModel;
        static Bot _botClient;

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

            _botClient = new Bot(appModel.BotConfiguration.BotToken, httpProxy);
            Console.ReadLine();
            _botClient.Stop();
        }

    }
}
