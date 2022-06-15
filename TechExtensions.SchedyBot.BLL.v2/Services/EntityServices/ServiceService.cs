using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.DLL.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices
{
    public class ServiceService : IServiceService
    {
        private readonly IDbRepository<Service> _repository;
        private readonly ILogger<ServiceService> _logger;
        public ServiceService(IDbRepository<Service> repository, ILogger<ServiceService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public List<Service> GetServices(Func<Service, bool> predicate)
        {
            try
            {
                var services =  _repository.Get(null)
                    .ToList();

                if (services == null)
                    _logger.LogWarning("Объект Bookings не найден");

                return services;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }
        public async Task CreateService(Service service)
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
        public Service GetService(Func<Service, bool> predicate)
        {
            try
            {
                var service =  _repository.Get(null)
                    .FirstOrDefault();

                if (service == null)
                    _logger.LogWarning("Объект Service не найден");

                return service;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }

        public async Task<Service?> GetByName(string serviceName)
        {try
            {
                var service =  await _repository.Get(s => s.Name == serviceName)
                    .FirstOrDefaultAsync();

                return service;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }

        public async Task Update(Service service)
        {
            try
            {
                service.CreationDate = DateTime.SpecifyKind(service.CreationDate, kind: DateTimeKind.Utc);
                _repository.Update(service);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }

        public async Task<List<Service>?> GetMainServices(string languageCode)
        {try
            {
                var services =  await _repository.Get(s => s.IsMain && s.LanguageCode == languageCode)
                    .ToListAsync();

                return services;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }
    }
}
