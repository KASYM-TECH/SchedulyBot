using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.DLL.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices
{
    /// <summary>
    /// TODO: Переименовать в UserClient и сущность тоже, сейчас готовая БД ругается
    /// </summary>
    public class ClientService : IClientService
    {
        private IDbRepository<Client> _repository;
        private ILogger<ClientService> _logger;

        public ClientService(IDbRepository<Client> repository, ILogger<ClientService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Create(Client user)
        {
            try
            {
                await _repository.CreateAsync(user);
                await _repository.SaveChangesAsync();
            }
            catch(Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }
        public async Task<Client?> GetById(int clientId)
        {
            try
            {
                //Убрал asNoTracking так как с ним EF не может Client к ServiceAndSpec привязать
                var user = await _repository.Get(x => x.Id == clientId)
                    .Include(d => d.Address)
                    .Include(d => d.ActionHistory)
                    .Include(d => d.Contact)
                    .Include(d => d.Schedules)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync();
                
                if (user == null)
                    _logger.LogInformation("Объект User не найден");

                return user;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }

        public async Task<Client?> GetByIdUntracked(int clientId)
        {
            try
            {
                var user = await _repository.Get(x => x.Id == clientId)
                    .Include(d => d.Address)
                    .Include(d => d.ActionHistory)
                    .Include(d => d.Contact)
                    .Include(d => d.Schedules)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (user == null)
                    _logger.LogInformation("Объект User не найден");

                return user;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }

        public async Task<Client?> GetUserByChatId(long chatId)
        {
            var user = await _repository.Get(x => x.ChatId == chatId)
                    .Include(d => d.Address)
                    .Include(d => d.ActionHistory)
                    .Include(d => d.Contact)
                    .Include(d => d.Schedules)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync();
            return user;
        }

        public async Task<List<Client>?> GetManyUsers(Func<Client, bool> predicate)
        {
            try
            {
                var users = await _repository.Get(null)
                    .Include(d => d.Address)
                    .Include(d => d.ActionHistory)
                    .Include(d => d.Contact)
                    .Include(d => d.Schedules)
                    .ToListAsync();

                if (users == null)
                    _logger.LogWarning("Объект User не найден");

                return users;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }
        public async Task Update(Client user)
        {
            try
            {
                _repository.Update(user);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }
    }
}
