using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using BotConsole.Interfaces;

namespace BotConsole.Jobs
{
    public class RegisterJob : IRegisterJob
    {
        ISchedulerBot bot;
        Random rnd;
        public RegisterJob(ISchedulerBot _bot)
        {
            bot = _bot;
            rnd = new Random();
        }

        public async Task CreateJob<T>(string prefix, IDictionary<string, object> map, DateTime dateOfReminder) where T : IJob
        {
            var job = JobBuilder.Create<T>().WithIdentity($"{prefix}{rnd.Next()}").UsingJobData(new JobDataMap(map)).Build();

            var trigger = TriggerBuilder.Create().WithIdentity($"{prefix}{rnd.Next()}").StartAt(dateOfReminder.ToUniversalTime())
                .Build();
            await bot.Scheduler.ScheduleJob(job, trigger);
        }
    }
}