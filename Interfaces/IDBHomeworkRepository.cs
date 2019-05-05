using System.Threading.Tasks;
using BotConsole.RequestModels;
using BotConsole.ResponseModels;

namespace BotConsole.Interfaces
{
    public interface IDBHomeworkRepository
    {
        Task<CreateHomeWorkResponse> SaveHomeworkInDB(CreateHomeWorkRequest data);

        GetNextHomeWorkResponse GetNextHomeWork(GetNextHomeWorkRequest data);

        Task<HomeworkResponse> GetHomeworkById(long id);
    }
}