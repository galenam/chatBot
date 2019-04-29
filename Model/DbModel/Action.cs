using System;
using System.Collections.Generic;

namespace BotConsole.Model
{
    public partial class Action
    {
        public Action()
        {
            ActionUserSend = new HashSet<ActionUserSend>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<ActionUserSend> ActionUserSend { get; set; }
    }
}
