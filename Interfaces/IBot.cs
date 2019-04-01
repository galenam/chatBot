using Telegram.Bot.Args;

namespace BotConsole.Interfaces
{
    public interface IBot
    {
        void Bot_OnMessage(object sender, MessageEventArgs e);
    }
}