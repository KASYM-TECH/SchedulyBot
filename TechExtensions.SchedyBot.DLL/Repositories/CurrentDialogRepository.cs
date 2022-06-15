using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.DLL.Repositories
{
    public sealed class CurrentDialogRepository : DbRepository<CurrentDialog>
    {
        public CurrentDialogRepository(SchedulyBotContext schedulyBotContext) : base(schedulyBotContext) { }
    }
}
