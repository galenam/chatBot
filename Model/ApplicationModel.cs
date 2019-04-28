namespace Model
{
    public class ApplicationModel
    {
        public BotConfiguration BotConfiguration { get; set; }
        public ProxyConfiguration ProxyConfiguration { get; set; }

        public ReminderSetting ReminderSettings { get; set; }
    }

    public class ReminderSetting
    {
        public int? Days { get; set; }
        public int? Hours { get; set; }
        public int? Minutes { get; set; }
    }

    public class BotConfiguration
    {
        public string BotToken { get; set; }
    }

    public class ProxyConfiguration
    {
        public string Url { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}