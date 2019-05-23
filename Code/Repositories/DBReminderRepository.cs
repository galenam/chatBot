using System;
using System.Threading.Tasks;
using BotConsole.Model;
using BotConsole.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BotConsole.RequestModels;
using BotConsole.ResponseModels;
using BotConsole.Extensions;

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
            if (data == null)
            {
                return null;
            }
            var result = new CreateHomeWorkReminderResponse() { Result = false };
            var reminder = await _context.UserHomeWorkReminder.FirstOrDefaultAsync(uhwr => uhwr.HomeWorkId == data.HomeWorkId && uhwr.UserId == data.UserId && uhwr.DateTimeSend == data.DateOfReminder.Ticks);
            if (reminder != null)
            {
                if (reminder.DateTimeSend.CompareTo(data.DateOfReminder.Ticks) != 0 && data.DateOfReminder.CompareTo(DateTime.Now) > 0)
                {
                    reminder.DateTimeSend = data.DateOfReminder.Ticks;
                    try
                    {
                        _context.UserHomeWorkReminder.Update(reminder);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogException(ex);
                    }
                }
                result.Id = reminder.Id;
            }
            else
            {
                try
                {
                    var addedReminder = new UserHomeWorkReminder { HomeWorkId = data.HomeWorkId, UserId = data.UserId, DateTimeSend = data.DateOfReminder.Ticks };
                    await _context.UserHomeWorkReminder.AddAsync(addedReminder);
                    await _context.SaveChangesAsync();
                    result.Result = true;
                    // todo проверить, что при добавлении новой напоминалки на дз, на которое еще нет напоминалок, возвращается Id
                    result.Id = addedReminder.Id;
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                }
            }
            return result;
        }

        public async Task<UpdatedReminderResponse> UpdateReminder(UpdatedReminderRequest request)
        {
            var result = new UpdatedReminderResponse { Result = false };
            if (request == null) return result;
            var reminder = await _context.UserHomeWorkReminder.FirstAsync(uhr => uhr.Id == request.Id);
            if (reminder == null) return result;
            reminder.IsSend = request.IsSend ? 1 : 0;
            reminder.DateTimeSend = request.DateTimeSend.Ticks;
            try
            {
                _context.Update<UserHomeWorkReminder>(reminder);
                await _context.SaveChangesAsync();
                result.Result = true;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            return result;
        }
    }
}