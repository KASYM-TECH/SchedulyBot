using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.DLL.Repositories
{
    public sealed class UpdateMessageRepository : DbRepository<UpdateMessage>
    {
        public UpdateMessageRepository(SchedulyBotContext schedulyBotContext) : base(schedulyBotContext) { }
    }
}
