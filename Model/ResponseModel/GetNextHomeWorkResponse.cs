using System;

namespace BotConsole.ResponseModels
{
    public class GetNextHomeWorkResponse
    {
        public long HomeWorkId { get; set; }
        public string Title { get; set; }
        public DateTime? DateOfReadyness { get; set; }
        public bool IsExisted { get; set; }
        public long UserId { get; set; }
    }
}