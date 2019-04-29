using System;
using System.Collections.Generic;

namespace BotConsole.Model
{
    public partial class UserHomeWork
    {
        public long Id { get; set; }
        public long Checked { get; set; }
        public long UserId { get; set; }
        public long HomeWorkId { get; set; }

        public HomeWork HomeWork { get; set; }
        public User User { get; set; }
    }
}
