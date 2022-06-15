using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling.Models;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.GreetingAndCustomizingUser.Branch0
{
    public class S7_ServicePriceInputStep : IDialogStep
    {
        private string _messageTag = "branch0step7mess";
        private string _errorMessage = "branch0step7err";

        public Dictionary<Type, string> NextStepTextTag { get; }
            = new Dictionary<Type, string> { { typeof(S8_SelectWeekDaysStep), null } };


        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IServiceAndSpecService _serviceAndSpecService;
        private readonly IUpdateContainer _container;
        private readonly BotReplyKeyboardMarkupHandler _markupHandler;

        public S7_ServicePriceInputStep(IMessageTranslationManger messageTranslationManger,
             IBotMessageManager botMessageSender,
           IServiceAndSpecService serviceAndSpecService)
        {
            _serviceAndSpecService = serviceAndSpecService;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
        }

        public async Task<DialogIteration?> HandleAnswerFromClient(string clientAnswer)
        {
            if (!AnswerTypeHandling.AnswerTypeGuard.AnswerIsCorrect<string>(AnswerType.IntegralNumber, clientAnswer))
            {
                var errorMessage = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _errorMessage);
                await _botMessageSender.Send(errorMessage);
                return currentDialog;
            }
            var serviceAndSpec =  _serviceAndSpecService.GetServiceAndSpec(s => s.User.Id == currentDialog.Client.Id);
            serviceAndSpec.Price = Int32.Parse(clientAnswer);
            await _serviceAndSpecService.Update(serviceAndSpec);

            currentDialog.CurrentStepType = NextStepTextTag.First().Key.ToString();
            return currentDialog;
        }
        public async Task SendReplyToUser()
        {
            var message = await GetMessage(currentDialog, translationCollectionName);
            var btns = await GetButtons();
            var markup = BotReplyKeyboardMarkupHandler.FormReplyKeyboardMarkupFromButtons(btns);
            await _botMessageSender.Send(message, markup);
        }

        public Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            throw new NotImplementedException();
        }

        public int TemplateId { get; } = 0;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 7;

        private async Task<List<string>> GetButtons()
        {
            var buttons = await StepService.AddBaseButtons(_messageTranslationManger);
            return buttons;
        }

        private async Task<string> GetMessage(CurrentDialog currentDialog, string translationCollectionName)
        {
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _messageTag);
            return message;
        }
    }
}
