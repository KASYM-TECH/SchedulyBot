using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechExtensions.SchedyBot.DLL.Entities
{
    /// <summary>
    /// Для создания точного маршрута по степам
    /// </summary>
    public class Step : BaseEntity
    {
        public Step(int templateId, int branchId, int stepId)
        {
            TemplateId = templateId;
            BranchId = branchId;
            StepId = stepId;
        }
        public Step() { }
        public int TemplateId { get; set; }
        public int BranchId { get; set; }
        public int StepId { get; set; }
    }
}
