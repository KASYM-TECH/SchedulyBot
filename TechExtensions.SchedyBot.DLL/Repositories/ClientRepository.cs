using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.DLL.Repositories
{
    public sealed class ClientRepository : DbRepository<Client>
    {
        public ClientRepository(SchedulyBotContext schedulyBotContext): base(schedulyBotContext) { }
    }
}
