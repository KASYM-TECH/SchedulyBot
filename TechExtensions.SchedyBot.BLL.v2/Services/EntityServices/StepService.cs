using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.DLL.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices
{
    public class StepService : IStepService
    {
        private readonly IDbRepository<Step> _repository;
        private readonly ILogger<StepService> _logger;
        public StepService(IDbRepository<Step> repository,
            ILogger<StepService> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task CreateMany(List<Step> steps)
        {
            try 
            {
               await _repository.CreateManyAsync(steps);
               await _repository.SaveChangesAsync();
            }
            catch(Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }
        public async Task DeleteMany(List<Step> steps)
        {
            try
            {
                _repository.DeleteMany(steps);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }
    }
}
