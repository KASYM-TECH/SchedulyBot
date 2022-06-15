using TechExtensions.SchedyBot.DLL.Entities;
using System.Threading.Tasks;

namespace TechExtensions.SchedyBot.BLL.BotDialogProcessing.Abstractions
{
    /// <summary>
    /// TODO: Подумать над названием
    /// </summary>
    public interface ITemplateServices
    {
        //public Task<CurrentDialog> StartNewTemplateIfCommandsMatch(string message, CurrentDialog currentDialog);
        public Task GoOneStepBack(CurrentDialog currentDialog);
        public Task SendMessageToClient(CurrentDialog currentDialog, IDialogStep step, IDialogTemplate dialogTemplate);
        //public Task<IDialogTemplate> GetDialogTemplates(Func<IDialogTemplate, bool> predicate = null);
        public IDialogTemplate GetDialogTemplateByStep(string stepClassName);

    }
}
