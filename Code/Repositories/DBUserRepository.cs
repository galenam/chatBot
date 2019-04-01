using System;
using System.Threading.Tasks;
using BotConsole.Model;
using BotConsole.Interfaces;
using Microsoft.Extensions.Logging;
using BotConsole.RequestModels;
using BotConsole.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace BotConsole.Code.Repositories
{
    public class DBUserRepository : IDBUserRepository
    {

        protected readonly BotDBContext _context;
        private ILogger<DBUserRepository> _logger;

        public DBUserRepository(BotDBContext context, ILogger<DBUserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CreateUserResponse> SaveUserInDB(CreateUserRequest data)
        {
            _logger.LogInformation("SaveUserInDB begin");
            if (data == null)
            {
                return null;
            }
            var result = new CreateUserResponse() { Result = false };

            if (await _context.User.FirstOrDefaultAsync(u => u.Name == data.Name && u.Surname == data.Surname && u.Login == data.Login) != null)
            {
                _logger.LogInformation("dublicate");
                return result;
            }
            try
            {
                await _context.User.AddAsync(new User { Name = data.Name, Surname = data.Surname, Login = data.Login });
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