using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.DLL.Repositories
{
    public sealed class ReviewRepository : DbRepository<Review>
    {
        public ReviewRepository(SchedulyBotContext schedulyBotContext) : base(schedulyBotContext) { }
    }
}
