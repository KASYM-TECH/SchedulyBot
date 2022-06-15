using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions
{
    public interface IDialogTemplate
    {
        public List<IDialogStep> DialogSteps { get; }
        public TemplateEnum TemplateId { get; }

        public (int branchId, int stepId)? GetBranchAndStepByState(TemplateStateEnum templateState);
        public string TranslationCollectionName { get; }
        Task OnCancel();
    }
}
