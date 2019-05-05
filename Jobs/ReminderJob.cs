
using System.Threading.Tasks;
using BotConsole.Const;
using BotConsole.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace BotConsole.Jobs
{

    public interface IReminderJob : IJob
    {
    }
    public class ReminderJob : IReminderJob
    {
        ILogger<ReminderJob> _logger;
        IBot _bot;
        IDBHomeworkRepository _iDBHomeworkRepository;
        public ReminderJob(ILogger<ReminderJob> logger, IBot bot, IDBHomeworkRepository iDBHomeworkRepository)
        {
            _logger = logger;
            _bot = bot;
            _iDBHomeworkRepository = iDBHomeworkRepository;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var parameters = context.JobDetail.JobDataMap;
            var userId = parameters.GetLongValue(ReminderJobConst.ChatId);
            var homeWorkId = parameters.GetLongValue(ReminderJobConst.HomeWordId);
            var homeWork = await _iDBHomeworkRepository.GetHomeworkById(homeWorkId);
            var textMessageToBot = ReminderJobConst.BeginMessage +
            (homeWork == null || string.IsNullOrEmpty(homeWork.Title) ? ReminderJobConst.DefaultHomeworkTitle : $"'{homeWork.Title}'");
            await _bot.Send(userId.ToString(), textMessageToBot);

        }
    }
}