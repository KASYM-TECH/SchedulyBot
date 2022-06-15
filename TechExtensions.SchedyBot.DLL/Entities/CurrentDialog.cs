using System.Collections.Generic;

namespace TechExtensions.SchedyBot.DLL.Entities
{
    public class CurrentDialog : BaseEntity
    {
        public long ChatId { get; set; }
        public CurrentDialogState State { get; set; }
        public int CurrentTemplateId { get; set; }
        public int CurrentBranchId { get; set; }
        public int CurrentStepId { get; set; }
        /// <summary>
        /// Если нужна специфическая последовательность стпепов. Можно даже из разных темплейтов
        /// </summary>
        public List<Step> StepRoute { get; set; }
        public List<CurrentDialogIteration> CurrentDialogIterations { get; set; }

        public CurrentDialog()
        {
            CurrentDialogIterations = new List<CurrentDialogIteration>();
            StepRoute = new List<Step>();
        }
    }
    
    public enum CurrentDialogState
    {
        Started = 0,
        InProgress = 1,
        Done = 2
    }
}
