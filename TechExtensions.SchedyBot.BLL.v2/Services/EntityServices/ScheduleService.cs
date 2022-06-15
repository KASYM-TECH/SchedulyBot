using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechExtensions.SchedyBot.BLL.v2.Extensions;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.DLL.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices
{
    public class ScheduleService : IScheduleService
    {
        private readonly IDbRepository<Schedule> _repository;
        private readonly ILogger<ScheduleService> _logger;
        public ScheduleService(IDbRepository<Schedule> repository, ILogger<ScheduleService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public List<Schedule> GetMany(Func<Schedule, bool> predicate)
        {
            try
            {
                var schedules = _repository.Get(null)
                    .ToList();
                if (schedules == null)
                    _logger.LogWarning("Объект Schedules не найден");

                return schedules;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }

        public async Task<Schedule?> GetByClientId(int clientId)
        {
            try
            {
                var schedule = await _repository.Get(s => s.ClientId == clientId)
                    .FirstOrDefaultAsync();
                return schedule;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }

        public async Task<List<Schedule>> GetManyByClientId(int clientId)
        {
            try
            {
                var schedules = await _repository.Get(s => s.ClientId == clientId)
                    .ToListAsync();
                return schedules;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }

        public Schedule GetSchedule(Func<Schedule, bool> predicate)
        {
            try
            {
                var schedule = _repository.Get(null)
                    .FirstOrDefault();
                if (schedule == null)
                    _logger.LogWarning("Объект Schedules не найден");

                return schedule;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                return null;
            }
        }
        public async Task Create(Schedule schedule)
        {
            try
            {
                await _repository.CreateAsync(schedule);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }
        
        public async Task Update(Schedule schedule)
        {
            try
            {
                _repository.Update(schedule);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }
        
        public async Task Delete(Schedule schedule)
        {
            try
            {
                _repository.Delete(schedule);
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }

        public async Task DeleteAllByClientId(int clientId)
        {
            await DeleteMany(await GetManyByClientId(clientId));
        }

        public async Task DeleteMany(IEnumerable<Schedule> schedules)
        {
            try
            {
                foreach (var schedule in schedules)
                    _repository.Delete(schedule);
                
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }

        public async Task<Schedule?> GetByClientIdAndWeekday(int executorId, DayOfWeek currentDateDayOfWeek)
        {
            var schedule = await _repository
                .Get(x => x.ClientId == executorId &&
                                x.WeekDay == currentDateDayOfWeek.ToWeekDay())
                .FirstOrDefaultAsync();
            return schedule;
        }
    }
}