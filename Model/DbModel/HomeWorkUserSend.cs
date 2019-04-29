using System;
using System.Collections.Generic;

namespace BotConsole.Model
{
    public partial class HomeWorkUserSend
    {
        public long Id { get; set; }
        public long HomeWorkId { get; set; }
        public long UserId { get; set; }
        public long IsSend { get; set; }

        public HomeWork HomeWork { get; set; }
        public User User { get; set; }
    }
}
