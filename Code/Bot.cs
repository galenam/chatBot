using System.Net;
using Model;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Args;
using System;
using BotConsole.Interfaces;
using BotConsole.Const;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using BotConsole.Jobs;
using Telegram.Bot.Types;

namespace BotConsole.Code
{
    public class Bot : IBot
    {
        static TelegramBotClient _botClient;
        static ILogger<Bot> _logger;
        static IDBHomeworkRepository _dbHomeworkRepository;
        static IDBReminderRepository _dbReminderRepository;
        static ApplicationModel _appModel;

        static IRegisterJob _registerJob;
        public Bot(ILogger<Bot> logger, IDBHomeworkRepository dbBHomeworkRepository, IOptions<ApplicationModel> options,
        IDBReminderRepository dbReminderRepository, IRegisterJob registerJob)
        {
            _logger = logger;
            _dbHomeworkRepository = dbBHomeworkRepository;
            _dbReminderRepository = dbReminderRepository;
            _appModel = options.Value;
            _registerJob = registerJob;
        }

        public void Start(string botToken, WebProxy httpProxy)
        {
            _botClient = new TelegramBotClient(botToken, httpProxy);
            _botClient.OnReceiveError += BotOnReceiveError;
            _botClient.OnMessage += Bot_OnMessage;
            _botClient.StartReceiving();
        }

        private void BotOnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            _logger.LogError($"InnerException={e.ApiRequestException.InnerException}, Message={e.ApiRequestException.Message}");
        }

        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                var me = await _botClient.GetMeAsync();
                var text = e.Message.Text;
                var resultText = string.Empty;
                if (string.IsNullOrEmpty(text))
                {
                    resultText = "Please, choose the command";
                    return;
                }
                //_logger.LogError(text);
                switch (text)
                {
                    case var included when text.ToLower().Contains(CommandConst.Reminder):
                        bool reminderCreated = await CreateReminder(e.Message.Chat.Id.ToString());
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + "Error message sending");
            }
        }

        private static async Task<bool> CreateReminder(string id)
        {
            // todo : исправить вызов и обработку результата
            var homeWork = _dbHomeworkRepository.GetNextHomeWork(new RequestModels.GetNextHomeWorkRequest { ChatId = id });
            if (homeWork == null || !homeWork.IsExisted) return false;
            DateTime dateOfReminder = DateTime.Now;
            if (homeWork.DateOfReadyness.HasValue)
            {
                dateOfReminder = homeWork.DateOfReadyness.Value.Subtract(new TimeSpan(_appModel?.ReminderSettings?.Days ?? 0,
                _appModel?.ReminderSettings?.Hours ?? 0, _appModel?.ReminderSettings?.Minutes ?? 0,
                0));
            }
            if (dateOfReminder.CompareTo(DateTime.Now) <= 0)
            {
                dateOfReminder = DateTime.Now.AddDays(_appModel?.ReminderSettings?.Days ?? 0)
                .AddHours(_appModel?.ReminderSettings?.Hours ?? 0)
                .AddMinutes(_appModel?.ReminderSettings?.Minutes ?? 0);
            }
            var saveResult = await _dbReminderRepository.SaveReminderInDB(new RequestModels.CreateHomeWorkReminderRequest
            {
                UserId = homeWork.UserId,
                HomeWorkId = homeWork.HomeWorkId,
                DateOfReminder = dateOfReminder
            });

            var dict = new Dictionary<string, object> { { ReminderJobConst.ChatId, id },
            { ReminderJobConst.HomeWordId, homeWork.HomeWorkId } };
            await _registerJob.CreateJob<IReminderJob>(ReminderJobConst.Reminder, dict, /* dateOfReminder*/DateTime.Now.AddSeconds(5));
            return saveResult != null && saveResult.Result;
        }

        public void Stop()
        {
            _botClient.StopReceiving();
        }

        public async Task Send(string chatId, string text)
        {
            Message message = await _botClient.SendTextMessageAsync(
              chatId: chatId, // or a chat id: 123456789
              text: text);
        }
    }
}