using System.Net;
using System.Threading.Tasks;
using Model;
using Telegram.Bot.Args;

namespace BotConsole.Interfaces
{
    public interface IBot
    {
        void Start();
        void Stop();

        Task<bool> Send(string chatId, string text);
    }
}