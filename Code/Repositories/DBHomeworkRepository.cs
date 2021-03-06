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
using BotConsole.Extensions;

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

        public async Task<HomeworkResponse> GetHomeworkById(long id)
        {
            if (id < 0) return new HomeworkResponse();
            var hw = await _context.HomeWork.FirstOrDefaultAsync(hws => hws.Id == id);
            return hw == null ? new HomeworkResponse() : new HomeworkResponse { Title = hw.Title, Id = id };
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
                    DateOfReadyness = HomeWork.DateOfReadyness.HasValue ? new DateTime(HomeWork.DateOfReadyness.Value) : default(DateTime?),
                    Title = HomeWork.Title,
                    UserId = Ids.UserId
                })
                .Where(hw => hw.DateOfReadyness.HasValue && DateTime.Now.CompareTo(hw.DateOfReadyness.Value) < 0)
                .OrderBy(hw => hw.DateOfReadyness)
                .FirstOrDefault();

            if (hws != null)
            {
                result.IsExisted = true;
                result.HomeWorkId = hws.HomeWorkId;
                result.Title = hws.Title;
                result.DateOfReadyness = hws.DateOfReadyness;
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
                _logger.LogException(ex);
                return result;
            }
        }
    }
}