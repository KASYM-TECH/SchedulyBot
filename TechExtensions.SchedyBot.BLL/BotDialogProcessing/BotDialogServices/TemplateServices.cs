using TechExtensions.SchedyBot.BLL.Extentions;
using TechExtensions.SchedyBot.DLL.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechExtensions.SchedyBot.BLL.BotDialogProcessing.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;

namespace TechExtensions.SchedyBot.BLL.BotDialogProcessing.BotDialogServices
{
    public class TemplateServices : ITemplateServices
    {
        private readonly IEnumerable<IDialogTemplate> _dialogTemplates;
        private readonly IMessageTranslationManger _messageTranslationManger;

        public TemplateServices(IEnumerable<IDialogTemplate> dialogTemplates, IMessageTranslationManger messageTranslationManger)
        {
            _dialogTemplates = dialogTemplates;
            _messageTranslationManger = messageTranslationManger;
        }

        public async Task GoOneStepBack(CurrentDialog currentDialog)
        {
            var dialogTemplate = GetDialogTemplateByStep(currentDialog.CurrentStepType);
            if (currentDialog.State == CurrentDialogState.BeingUsedByInlineKeyboard)
            {
                currentDialog.CurrentInlineKeyboard = null;
                currentDialog.State = CurrentDialogState.BeingUsedByTemplate;
            }
            var previousStepName = currentDialog.StepTypeHistory.ElementAt(currentDialog.StepTypeHistory.Count - 2);
            var previousStep = dialogTemplate.GetStepByClassName(previousStepName);
            currentDialog.StepTypeHistory.RemoveAt(currentDialog.StepTypeHistory.Count - 1);
            await SendMessageToClient(currentDialog, previousStep, dialogTemplate);
            currentDialog.StepTypeHistory = currentDialog.StepTypeHistory.Distinct().ToList();
            currentDialog.CurrentStepType = previousStep.GetStepClassName();
        }

        public async Task SendMessageToClient(CurrentDialog currentDialog, IDialogStep step, IDialogTemplate dialogTemplate)
        {
            currentDialog.CurrentStepType = step.GetStepClassName();
            if (currentDialog.CurrentInlineKeyboard == null)
                LogStepHistory(currentDialog, step);
            await step.SendReplyToUser(currentDialog, dialogTemplate.TranslationCollectionName);
        }
        public void LogStepHistory(CurrentDialog currentDialog, IDialogStep step)
        {
            if (currentDialog.StepTypeHistory == null)
                currentDialog.StepTypeHistory = new List<string> { step.GetStepClassName() };
            else
                currentDialog.StepTypeHistory.Add(step.GetStepClassName());
            if (currentDialog.StepTypeHistory.Count > 5)
                currentDialog.StepTypeHistory.RemoveAt(0);
        }
        public IDialogTemplate GetDialogTemplateByStep(string stepClassName)
        {
            foreach (var template in _dialogTemplates)
            {
                var step = template.GetStepByClassName(stepClassName);
                if (step?.DialogTemplateType == template.GetType() && template is not null)
                    return template;
            }
            return null;
        }
    }
}
