using System;
using System.Threading.Tasks;
using BotConsole.Model;
using BotConsole.Interfaces;
using Microsoft.Extensions.Logging;
using BotConsole.RequestModels;
using BotConsole.ResponseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

namespace BotConsole.Code.Repositories
{
    public class DBHomeworkRepository : IDBHomeworkRepository
    {

        protected readonly BotDBContext _context;
        private ILogger<DBHomeworkRepository> _logger;

        public DBHomeworkRepository(BotDBContext context, ILogger<DBHomeworkRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public GetNextHomeWorkResponse GetNextHomeWork(GetNextHomeWorkRequest data)
        {
            var result = new GetNextHomeWorkResponse { IsExisted = false };
            if (data == null || string.IsNullOrEmpty(data.ChatId)) return result;

            var hws = _context.User.Join(_context.UserHomeWork,
                User => User.Id, UserHomeWork => UserHomeWork.UserId,
                (User, UserHomeWork) => new
                {
                    ChatId = User.ChatId,
                    HomeWorkId = UserHomeWork.HomeWorkId,
                    UserId = User.Id
                }).Join(_context.HomeWork, Ids => Ids.HomeWorkId, HomeWork => HomeWork.Id,
                (Ids, HomeWork) => new
                {
                    ChatId = Ids.ChatId,
                    HomeWorkId = HomeWork.Id,
                    DateOfReadyness = HomeWork.DateOfReadyness,
                    Title = HomeWork.Title,
                    UserId = Ids.UserId
                }).FirstOrDefault();
            if (hws != null)
            {
                result.IsExisted = true;
                result.HomeWorkId = hws.HomeWorkId;
                result.Title = hws.Title;
                result.DateOfReadyness = hws.DateOfReadyness.HasValue ? new DateTime(hws.DateOfReadyness.Value) : default;
                result.UserId = hws.UserId;
            }

            return result;
        }

        public async Task<CreateHomeWorkResponse> SaveHomeworkInDB(CreateHomeWorkRequest data)
        {
            _logger.LogInformation("SaveHomeworkInDB begin");
            if (data == null)
            {
                return null;
            }
            var result = new CreateHomeWorkResponse() { Result = false };

            if (await _context.HomeWork.FirstOrDefaultAsync(u => u.Title == data.Title && u.DateOfReadyness == data.DateOfReadyness.Ticks) != null)
            {
                _logger.LogInformation("dublicate");
                return result;
            }
            try
            {
                await _context.HomeWork.AddAsync(new HomeWork { Title = data.Title, DateOfReadyness = data.DateOfReadyness.Ticks });
                await _context.SaveChangesAsync();
                result.Result = true;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "saving db error");
                return result;
            }
        }
    }
}