using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.DLL.Repositories
{
    public sealed class BookingRepository : DbRepository<Booking>
    {
        public BookingRepository(SchedulyBotContext schedulyBotContext) : base(schedulyBotContext) { }
    }
}
