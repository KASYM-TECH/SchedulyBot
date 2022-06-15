using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.DLL.Repositories
{
    public sealed class ScheduleRepository : DbRepository<Schedule>
    {
        public ScheduleRepository(SchedulyBotContext schedulyBotContext) : base(schedulyBotContext) { }
    }
}
