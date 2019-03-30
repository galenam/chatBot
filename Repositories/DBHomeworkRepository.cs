using System;
using System.Threading.Tasks;
using Bot.Model;
using Interfaces;
using Microsoft.Extensions.Logging;
using RequestModels;
using ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace Code
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