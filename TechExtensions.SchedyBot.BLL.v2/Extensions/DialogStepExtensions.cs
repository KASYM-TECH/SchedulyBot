using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;

namespace TechExtensions.SchedyBot.BLL.v2.Extensions;

public static class DialogStepExtensions
{
    public static bool Equal(this IDialogStep dialogStep, IDialogStep anotherStep)
    {
        if (dialogStep.TemplateId == anotherStep.TemplateId &&
            dialogStep.BranchId == anotherStep.BranchId &&
            dialogStep.StepId == anotherStep.StepId)
            return true;
        
        return false;
    }

    public static DialogIteration NextStep(this IDialogStep dialogStep)
    {
        return new DialogIteration(dialogStep.TemplateId, dialogStep.BranchId, dialogStep.StepId + 1);
    }

    public static DialogIteration NextStepWithoutMessage(this IDialogStep dialogStep)
    {
        return new DialogIteration(dialogStep.TemplateId, dialogStep.BranchId, dialogStep.StepId + 1, false);
    }

    public static DialogIteration NextBranch(this IDialogStep dialogStep)
    {
        return new DialogIteration(dialogStep.TemplateId, dialogStep.BranchId + 1, 0);
    }

    public static DialogIteration CurrentStep(this IDialogStep dialogStep)
    {
        return new DialogIteration(dialogStep.TemplateId, dialogStep.BranchId, dialogStep.StepId);
    }

    public static DialogIteration CurrentStepWithoutMessage(this IDialogStep dialogStep)
    {
        return new DialogIteration(dialogStep.TemplateId, dialogStep.BranchId, dialogStep.StepId, false);
    }
}