using System;
namespace BotConsole.RequestModels
{
    public class CreateHomeWorkRequest
    {
        public string Title { get; set; }
        public DateTime DateOfReadyness { get; set; }
    }
}