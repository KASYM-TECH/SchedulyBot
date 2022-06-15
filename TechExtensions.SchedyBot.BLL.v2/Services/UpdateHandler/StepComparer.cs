using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Services.UpdateHandler
{
    class StepComparer : IComparer<Step>
    {
        public int Compare(Step? s1, Step? s2)
        {
            if (s1 is null || s2 is null)
                throw new ArgumentException("Некорректное значение параметра");
            return s1.StepId - s2.StepId;
        }
    }
}
