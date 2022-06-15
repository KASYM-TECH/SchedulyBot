using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.DLL.Repositories
{
    public sealed class StepRepository : DbRepository<Step>
    {
        public StepRepository(SchedulyBotContext schedulyBotContext) : base(schedulyBotContext) { }
    }
}
