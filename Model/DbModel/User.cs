using System;
using System.Collections.Generic;

namespace BotConsole.Model
{
    public partial class User
    {
        public User()
        {
            ActionUserSend = new HashSet<ActionUserSend>();
            HomeWorkUserSend = new HashSet<HomeWorkUserSend>();
            UserHomeWork = new HashSet<UserHomeWork>();
            UserHomeWorkReminder = new HashSet<UserHomeWorkReminder>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ChatId { get; set; }

        public ICollection<ActionUserSend> ActionUserSend { get; set; }
        public ICollection<HomeWorkUserSend> HomeWorkUserSend { get; set; }
        public ICollection<UserHomeWork> UserHomeWork { get; set; }
        public ICollection<UserHomeWorkReminder> UserHomeWorkReminder { get; set; }
    }
}
