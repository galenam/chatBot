using System.Threading.Tasks;
using BotConsole.RequestModels;
using BotConsole.ResponseModels;

namespace BotConsole.Interfaces
{
    public interface IDBUserRepository
    {
        Task<CreateUserResponse> SaveUserInDB(CreateUserRequest data);
    }
}