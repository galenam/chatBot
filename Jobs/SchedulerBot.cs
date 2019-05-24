using System.Collections.Specialized;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using BotConsole.Interfaces;
using Microsoft.Extensions.Logging;
using BotConsole.Extensions;

namespace BotConsole.Jobs
{
    public class SchedulerBot : ISchedulerBot
    {
        static ILogger<SchedulerBot> _logger { get; set; }
        public IScheduler Scheduler { get; set; }


        public SchedulerBot(ILogger<SchedulerBot> logger)
        {
            _logger = logger;
        }

        public async Task<IScheduler> StartScheduler()
        {
            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" },
                {"quartz.jobStore.type","Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" },
                { "quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.SQLiteDelegate, Quartz"},
                {"quartz.jobStore.tablePrefix","QRTZ_"},
                {"quartz.jobStore.dataSource", "default"},
                {"quartz.dataSource.default.connectionString", "Data Source=F:\\\\dev\\\\BotConsole\\\\db\\\\botdb.db;"},
                {"quartz.dataSource.default.provider", "SQLite-Microsoft"},
                {"quartz.jobStore.useProperties", "false"}
            };

            var factory = new StdSchedulerFactory(props);
            try
            {
                Scheduler = await factory.GetScheduler();
            }
            catch (System.Exception ex)
            {
                _logger.LogException(ex);
            }

            await Scheduler.Start();
            return Scheduler;
        }
    }
}