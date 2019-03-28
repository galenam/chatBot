namespace Model
{
    public class ApplicationModel
    {
        public BotConfiguration BotConfiguration { get; set; }
        public ProxyConfiguration ProxyConfiguration { get; set; }
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