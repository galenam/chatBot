using System;

namespace BotConsole.ResponseModels
{
    public class UpdatedReminderRequest
    {
        public long Id { get; set; }
        public bool IsSend { get; set; }
        public DateTime DateTimeSend { get; set; }
    }
}