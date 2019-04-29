using System;
namespace BotConsole.RequestModels
{
    public class CreateHomeWorkReminderRequest
    {
        public long UserId { get; set; }
        public long HomeWorkId { get; set; }
        public DateTime DateOfReminder { get; set; }
    }
}