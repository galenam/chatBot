using System.Collections.Specialized;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using BotConsole.Interfaces;

namespace BotConsole.Jobs
{
    public class SchedulerBot : ISchedulerBot
    {
        public IScheduler Scheduler { get; set; }

        public async Task<IScheduler> StartScheduler()
        {
            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            var factory = new StdSchedulerFactory(props);

            Scheduler = await factory.GetScheduler();
            await Scheduler.Start();
            return Scheduler;
        }
    }
}