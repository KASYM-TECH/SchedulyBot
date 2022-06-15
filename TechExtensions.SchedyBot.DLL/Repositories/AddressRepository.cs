using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.DLL.Repositories
{
    public sealed class AddressRepository : DbRepository<Address>
    {
        public AddressRepository(SchedulyBotContext schedulyBotContext) : base(schedulyBotContext) { }
    }
}
