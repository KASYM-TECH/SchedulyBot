using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.DLL.Repositories
{
    public sealed class ContactRepository : DbRepository<Contact>
    {
        public ContactRepository(SchedulyBotContext schedulyBotContext) : base(schedulyBotContext) { }
    }
}