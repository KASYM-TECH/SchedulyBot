using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Enums;

namespace TechExtensions.SchedyBot.BLL.v2.Services.CurrentDialogNavigator.DI;

public interface ICurrentDialogNavigator
{
    Task HandleCurrentStep();
    Task<bool> GoOneStepBackIfNeed();
    Task<bool> CancelIfNeed();
    Task Cancel(CancelReason reason);
}