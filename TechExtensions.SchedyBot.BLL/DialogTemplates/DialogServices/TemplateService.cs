using TechExtensions.SchedyBot.BLL.DialogTemplates.DialogServices.DI;
using TechExtensions.SchedyBot.BLL.DialogTemplates.Enums;

namespace TechExtensions.SchedyBot.BLL.DialogTemplates.DialogServices;

public sealed class TemplateService : ITemplateService
{
    private readonly IEnumerable<IDialogTemplate> _dialogTemplates;

    public TemplateService(IEnumerable<IDialogTemplate> dialogTemplates)
    {
        _dialogTemplates = dialogTemplates;
    }

    public IDialogTemplate? GetDialogTemplateByStep(string? stepClassName)
    {
        if (stepClassName.IsNullOrEmpty())
            return null;
        
        var result = _dialogTemplates.FirstOrDefault(t => t.GetStepByClassName(stepClassName!).DialogTemplateType == t.GetType());
        return result;
    }

    public IDialogTemplate? GetDialogTemplateByCommand(string? command)
    {
        if (command.IsNullOrEmpty())
            return null;
        
        var result = _dialogTemplates.FirstOrDefault(t => t.Command == command);
        return result;
    }

    public IDialogTemplate? GetDialogTemplateByEnum(TemplateEnum templateEnum)
    {
        var result = _dialogTemplates
            .FirstOrDefault(t => t.TemplateId == templateEnum);
        return result;
    }
}