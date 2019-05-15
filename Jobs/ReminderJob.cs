
using System;
using System.Threading.Tasks;
using BotConsole.Const;
using BotConsole.Interfaces;
using BotConsole.ResponseModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model;
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
        ApplicationModel _appModel;

        IDBReminderRepository _iDBReminderRepository;
        public ReminderJob(ILogger<ReminderJob> logger, IBot bot, IDBHomeworkRepository iDBHomeworkRepository,
        IOptions<ApplicationModel> options, IDBReminderRepository iDBReminderRepository)
        {
            _logger = logger;
            _bot = bot;
            _iDBHomeworkRepository = iDBHomeworkRepository;
            _appModel = options.Value;
            _iDBReminderRepository = iDBReminderRepository;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var parameters = context.JobDetail.JobDataMap;
            var chatId = parameters.GetLongValue(ReminderJobConst.ChatId);
            var homeworkReminderId = parameters.GetLongValue(ReminderJobConst.HomeworkReminderId);
            var homeWorkId = parameters.GetLongValue(ReminderJobConst.HomeWordId);
            var homeWork = await _iDBHomeworkRepository.GetHomeworkById(homeWorkId);
            var textMessageToBot = ReminderJobConst.BeginMessage +
                (homeWork == null || string.IsNullOrEmpty(homeWork.Title) ? ReminderJobConst.DefaultHomeworkTitle : $"'{homeWork.Title}'");

            for (var i = 0; i++ < _appModel.ReminderSettings.RepeatNumber;)
            {
                if (await _bot.Send(chatId.ToString(), textMessageToBot))
                {
                    await _iDBReminderRepository.UpdateReminder(new UpdatedReminderRequest { IsSend = true, DateTimeSend = DateTime.Now, Id = homeworkReminderId });
                    break;
                }
            }
        }
    }
}