namespace BotConsole.RequestModels
{
    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Login { get; set; }
    }
}