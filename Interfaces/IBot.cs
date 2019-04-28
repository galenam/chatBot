using System.Net;
using System.Threading.Tasks;
using Telegram.Bot.Args;

namespace BotConsole.Interfaces
{
    public interface IBot
    {
        void Start(string botToken, WebProxy httpProxy);
        void Stop();

        Task Send(string chatId, string text);
    }
}