using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI
{
    public interface IScheduleService
    {
        public Task Create(Schedule schedule);
        public  Task Update(Schedule schedule);
        public Schedule GetSchedule(Func<Schedule, bool> predicate);
        public List<Schedule> GetMany(Func<Schedule, bool> predicate);
        Task<Schedule?> GetByClientId(int clientId);
        Task<List<Schedule>> GetManyByClientId(int clientId);
        Task Delete(Schedule schedules);
        Task DeleteAllByClientId(int clientId);
        Task DeleteMany(IEnumerable<Schedule> schedules);
        Task<Schedule?> GetByClientIdAndWeekday(int executorId, DayOfWeek currentDateDayOfWeek);
    }
}
