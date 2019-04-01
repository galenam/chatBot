using System.Threading.Tasks;
using Quartz;

namespace BotConsole.Interfaces
{
    public interface ISchedulerBot
    {
        IScheduler Scheduler { get; set; }
        Task<IScheduler> StartScheduler();
    }
}