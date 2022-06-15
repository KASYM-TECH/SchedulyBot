using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.DLL.Repositories
{
    public sealed class ActionHistoryRepository : DbRepository<ActionHistory>
    {
        public ActionHistoryRepository(SchedulyBotContext schedulyBotContext) : base(schedulyBotContext) { }
    }
}
