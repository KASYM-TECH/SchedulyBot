using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch0;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;

public sealed class DialogIteration
{
    public DialogIteration(int templateId, int branchId, int stepId, bool sendMessage = true)
    {
        TemplateId = templateId;
        BranchId = branchId;
        StepId = stepId;
        SendMessage = sendMessage;
    }

    public DialogIteration()
    {
        
    }

    public override bool Equals(Object? dialogIterationObj)
    {
        if (dialogIterationObj == null)
            return false;
        var dialogIteration = dialogIterationObj as DialogIteration;
        if (dialogIteration == null)
            return false;
        if (dialogIteration.TemplateId == TemplateId && dialogIteration.BranchId == BranchId && dialogIteration.StepId == StepId)
            return true;
        return false;
    }
    public override int GetHashCode()
    {
        var hashCode = (TemplateId, BranchId, StepId).GetHashCode();
        return hashCode;
    }
    public int TemplateId { get; set; }
    public int BranchId { get; set; }
    public int StepId { get; set; }

    public bool SendMessage { get; set; } = true;

}