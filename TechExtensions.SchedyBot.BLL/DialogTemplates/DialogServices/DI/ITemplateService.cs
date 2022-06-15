using TechExtensions.SchedyBot.BLL.DialogTemplates.Enums;

#nullable enable
namespace TechExtensions.SchedyBot.BLL.DialogTemplates.DialogServices.DI;

public interface ITemplateService
{
    /// <summary>
    /// Достань темплейт по наименованию шага TODO: почему class?
    /// </summary>
    /// <param name="stepClassName"></param>
    /// <returns></returns>
    IDialogTemplate? GetDialogTemplateByStep(string? stepClassName);
    
    /// <summary>
    /// Достань темплейт по наименованию команды
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    IDialogTemplate? GetDialogTemplateByCommand(string? command);
    
    /// <summary>
    /// Достань темплейт по енаму
    /// </summary>
    /// <param name="templateEnum"></param>
    /// <returns></returns>
    IDialogTemplate? GetDialogTemplateByEnum(TemplateEnum templateEnum);
}