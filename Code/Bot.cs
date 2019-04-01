using System.Net;
using Model;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace BotConsole.Code
{
    public class Bot
    {
        static TelegramBotClient _botClient;
        public Bot(ApplicationModel appSettings)
        {
            NetworkCredential credentials = null;
            var userName = appSettings.ProxyConfiguration.UserName;
            var password = appSettings.ProxyConfiguration.Password;
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                credentials = new NetworkCredential(userName, password);
            var httpProxy = new WebProxy(appSettings.ProxyConfiguration.Url, appSettings.ProxyConfiguration.Port)
            {
                Credentials = credentials
            };

            _botClient = new TelegramBotClient(appSettings.BotConfiguration.BotToken, httpProxy);

            _botClient.OnMessage += Bot_OnMessage;
            _botClient.StartReceiving();
        }

        public async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                //Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

                await _botClient.SendTextMessageAsync(
                  chatId: e.Message.Chat,
                  text: "You said:\n" + e.Message.Text
                );
            }
        }
    }

}