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
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

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
            var servicesProvider = BuildDi();
            _botClient = servicesProvider.GetRequiredService<Bot>();
            _botClient.Start(appModel.BotConfiguration.BotToken, httpProxy);
            Console.ReadLine();
            _botClient.Stop();
            NLog.LogManager.Shutdown();
        }

        private static ServiceProvider BuildDi()
        {
            return new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    builder.AddNLog(new NLogProviderOptions
                    {
                        CaptureMessageTemplates = true,
                        CaptureMessageProperties = true
                    });
                })
                .AddSingleton<Bot>()
                .BuildServiceProvider();
        }

    }
}
