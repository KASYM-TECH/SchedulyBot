using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.DLL.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;
using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices
{
    public class ServiceAndSpecService : IServiceAndSpecService
    {
        private readonly IDbRepository<ServiceAndSpec> _repository;
        private readonly ILogger<ServiceAndSpecService> _logger;
        public ServiceAndSpecService(IDbRepository<ServiceAndSpec> repository, ILogger<ServiceAndSpecService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Create(ServiceAndSpec service)
        {
            try
            {
                await _repository.CreateAsync(service);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }

        public ServiceAndSpec GetServiceAndSpec(Func<ServiceAndSpec, bool> predicate)
        {
            try
            {
                var serviceKindSpec = _repository.Get(null)
                    .Include(s => s.User)
                    .Include(s => s.Service)
                    .FirstOrDefault();

                if (serviceKindSpec == null)
                    _logger.LogWarning("Объект ServiceAndSpec не найден");

                return serviceKindSpec;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }

        public async Task<ServiceAndSpec?> GetByClientId(int clientId)
        {
            try
            {
                var serviceAndSpec = await _repository.Get(x => x.User.Id == clientId)
                    .Include(x => x.Service)
                    .Include(x => x.User)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();

                return serviceAndSpec;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Не удалось достать услугу со спецификациями по клиенту");
                Console.WriteLine(e);
                throw;
            }
        }
        public async Task<ServiceAndSpec?> GetByClientIdUntracked(int clientId)
        {
            try
            {
                var serviceAndSpec = await _repository.Get(x => x.User.Id == clientId)
                    .Include(x => x.Service)
                    .Include(x => x.User)
                    .AsNoTracking()
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();

                return serviceAndSpec;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Не удалось достать услугу со спецификациями по клиенту");
                Console.WriteLine(e);
                throw;
            }
        }
        public async Task<ServiceAndSpec?> GetById(int sasId)
        {
            try
            {
                var serviceAndSpec = await _repository.Get(x => x.Id == sasId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                return serviceAndSpec;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Не удалось достать услугу со спецификациями по клиенту");
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<List<ServiceAndSpec>> GetManyByClientId(int clientId)
        {
            try
            {
                var serviceKindSpecs = await _repository.Get(x => x.User.Id == clientId)
                    .Include(s => s.User)
                    .Include(s => s.Service)
                    .ToListAsync();

                return serviceKindSpecs;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }
        public async Task Update(ServiceAndSpec serviceAndSpec)
        {
            try
            {
                serviceAndSpec.CreationDate = DateTime.SpecifyKind(serviceAndSpec.CreationDate, kind: DateTimeKind.Utc);

                _repository.Update(serviceAndSpec);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }

        public async Task Delete(ServiceAndSpec serviceAndSpec)
        {
            try
            {
                _repository.Delete(serviceAndSpec);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }
    }
}
