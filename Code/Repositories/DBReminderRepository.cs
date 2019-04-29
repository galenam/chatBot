using System;
using System.Threading.Tasks;
using BotConsole.Model;
using BotConsole.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BotConsole.RequestModels;
using BotConsole.ResponseModels;

namespace BotConsole.Code.Repositories
{
    public class DBReminderRepository : IDBReminderRepository
    {
        protected readonly BotDBContext _context;
        private ILogger<DBReminderRepository> _logger;

        public DBReminderRepository(BotDBContext context, ILogger<DBReminderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CreateHomeWorkReminderResponse> SaveReminderInDB(CreateHomeWorkReminderRequest data)
        {
            _logger.LogInformation("SaveReminderInDB begin");
            if (data == null)
            {
                return null;
            }
            var result = new CreateHomeWorkReminderResponse() { Result = false };

            if (await _context.UserHomeWorkReminder.FirstOrDefaultAsync(uhwr => uhwr.HomeWorkId == data.HomeWorkId && uhwr.UserId == data.UserId && uhwr.DateTimeSend == data.DateOfReminder.Ticks) != null)
            {
                _logger.LogInformation("dublicate");
                return result;
            }
            try
            {
                await _context.UserHomeWorkReminder.AddAsync(new UserHomeWorkReminder { HomeWorkId = data.HomeWorkId, UserId = data.UserId, DateTimeSend = data.DateOfReminder.Ticks });
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