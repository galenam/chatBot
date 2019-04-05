using System.Net;
using Telegram.Bot.Args;

namespace BotConsole.Interfaces
{
    public interface IBot
    {
        void Start(string botToken, WebProxy httpProxy);
        void Stop();
    }
}