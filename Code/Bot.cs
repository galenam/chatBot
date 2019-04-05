using System.Net;
using Model;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Args;
using System;
using BotConsole.Interfaces;
using BotConsole.Const;
using Microsoft.Extensions.Logging;

namespace BotConsole.Code
{
    public class Bot : IBot
    {
        static TelegramBotClient _botClient;
        ILogger<Bot> _logger;
        public Bot(ILogger<Bot> logger)
        {
            _logger = logger;
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
            throw new NotImplementedException();
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
                switch (text)
                {
                    case var included when text.ToLower().Contains(CommandConst.Reminder):
                        // todo: добавить напоминания на всех будущие (???) дз 
                        break;
                }

                //System.Console.WriteLine($"Hello! My name is {me.FirstName}");
                //             Message message = await _botClient.SendTextMessageAsync(
                //   chatId: e.Message.Chat.Id, // or a chat id: 123456789
                //   text: e.Message.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + "Error message sending");
            }
        }

        public void Stop()
        {
            _botClient.StopReceiving();
        }
    }

}