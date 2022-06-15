using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;

namespace TechExtensions.SchedyBot.BLL.v2.Services;

public class TemplateService
{
    private readonly IEnumerable<IDialogTemplate> _dialogTemplates;
    
    public TemplateService(IEnumerable<IDialogTemplate> dialogTemplates)
    {
        _dialogTemplates = dialogTemplates;
    }
    
    public IDialogTemplate? GetTemplateByEnum(TemplateEnum templateEnum)
    {
        var template = _dialogTemplates.FirstOrDefault(t => t.TemplateId == templateEnum);
        return template;
    }
}