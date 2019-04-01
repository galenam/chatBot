using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quartz;

namespace BotConsole.Interfaces
{
    public interface IRegisterJob
    {
        Task CreateJob<T>(string jobName, IDictionary<string, object> map, DateTime dateOfReminder) where T : IJob;
        //Task TempMethod();
    }
}