using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.DLL.Repositories
{
    public sealed class ServiceAndSpecRepository : DbRepository<ServiceAndSpec>
    {
        public ServiceAndSpecRepository(SchedulyBotContext schedulyBotContext) : base(schedulyBotContext) { }
    }
}
