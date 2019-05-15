using System.Threading.Tasks;
using BotConsole.RequestModels;
using BotConsole.ResponseModels;

namespace BotConsole.Interfaces
{
    public interface IDBReminderRepository
    {
        Task<CreateHomeWorkReminderResponse> SaveReminderInDB(CreateHomeWorkReminderRequest data);

        Task<UpdatedReminderResponse> UpdateReminder(UpdatedReminderRequest request);
    }
}