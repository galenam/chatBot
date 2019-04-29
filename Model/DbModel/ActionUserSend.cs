using System;
using System.Collections.Generic;

namespace BotConsole.Model
{
    public partial class ActionUserSend
    {
        public long Id { get; set; }
        public long ActionId { get; set; }
        public long UserId { get; set; }

        public Action Action { get; set; }
        public User User { get; set; }
    }
}
