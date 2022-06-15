using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.DLL.Repositories
{
    public sealed class BookingInConfirmationRepository : DbRepository<BookingInConfirmation>
    {
        public BookingInConfirmationRepository(SchedulyBotContext schedulyBotContext) : base(schedulyBotContext) { }
    }
}
