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
using BotConsole.Code.Repositories;
using BotConsole.Interfaces;
using BotConsole.Jobs;
using BotConsole.Model;
using Microsoft.EntityFrameworkCore;
using Quartz.Core;

namespace BotConsole
{
    class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            var section = configuration.GetSection("ApplicationModel");
            var appModel = section.Get<ApplicationModel>();

            NetworkCredential credentials = null;
            var userName = appModel.ProxyConfiguration.UserName;
            var password = appModel.ProxyConfiguration.Password;
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                credentials = new NetworkCredential(userName, password);
            var httpProxy = new WebProxy(appModel.ProxyConfiguration.Url, appModel.ProxyConfiguration.Port)
            {
                Credentials = credentials
            };
            var connecionString = configuration.GetConnectionString("BotDBConnection");
            var servicesProvider = BuildDi(connecionString, section);
            var _botClient = servicesProvider.GetRequiredService<IBot>();
            _botClient.Start(appModel.BotConfiguration.BotToken, httpProxy);

            var schedBor = servicesProvider.GetRequiredService<ISchedulerBot>();
            var logger = servicesProvider.GetRequiredService<ILogger<DIJobFactory>>();
            schedBor.StartScheduler();
            schedBor.Scheduler.JobFactory = new DIJobFactory(logger, servicesProvider);

            Console.ReadLine();
            _botClient.Stop();
            NLog.LogManager.Shutdown();
        }

        private static ServiceProvider BuildDi(string connectionString, IConfigurationSection section)
        {
            var sCollection = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    builder.AddNLog(new NLogProviderOptions
                    {
                        CaptureMessageTemplates = true,
                        CaptureMessageProperties = true
                    });
                })
                .AddSingleton<IBot, Bot>()
                .AddScoped<IDBHomeworkRepository, DBHomeworkRepository>()
                .AddScoped<IDBReminderRepository, DBReminderRepository>()
                .AddScoped<IRegisterJob, RegisterJob>()
                .AddScoped<IReminderJob, ReminderJob>()
                .AddDbContext<BotDBContext>(options => options.UseSqlite(connectionString))
                .AddSingleton<ISchedulerBot, SchedulerBot>();
            sCollection.Configure<ApplicationModel>(section);
            return sCollection.BuildServiceProvider();
        }

    }
}
