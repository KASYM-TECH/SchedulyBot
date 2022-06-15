using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI
{
    public interface IStepService
    {
        public Task CreateMany(List<Step> steps);
        public Task DeleteMany(List<Step> steps);
    }
}
