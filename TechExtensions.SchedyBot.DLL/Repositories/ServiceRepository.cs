using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.DLL.Repositories
{
    public sealed class ServiceRepository : DbRepository<Service>
    {
        public ServiceRepository(SchedulyBotContext schedulyBotContext) : base(schedulyBotContext) { }
    }
}
