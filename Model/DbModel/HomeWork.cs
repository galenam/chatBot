using System;
using System.Collections.Generic;

namespace BotConsole.Model
{
    public partial class HomeWork
    {
        public HomeWork()
        {
            HomeWorkUserSend = new HashSet<HomeWorkUserSend>();
            UserHomeWork = new HashSet<UserHomeWork>();
            UserHomeWorkReminder = new HashSet<UserHomeWorkReminder>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public long? DateOfReadyness { get; set; }

        public ICollection<HomeWorkUserSend> HomeWorkUserSend { get; set; }
        public ICollection<UserHomeWork> UserHomeWork { get; set; }
        public ICollection<UserHomeWorkReminder> UserHomeWorkReminder { get; set; }
    }
}
