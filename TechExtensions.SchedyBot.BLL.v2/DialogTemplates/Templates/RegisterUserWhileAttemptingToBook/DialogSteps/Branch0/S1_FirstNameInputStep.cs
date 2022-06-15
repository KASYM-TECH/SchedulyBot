using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling.Models;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.RegisterUserWhileAttemptingToBook.DialogSteps.Branch0
{
    public class S1_FirstNameInputStep : IDialogStep
    {

        public Type DialogTemplateType { get; } = typeof(RegisterUserWhileAttemptingToBookTemplate);
        private string _messageTag = "branch0step1mess";
        private string _errorMessage = "branch0EnterErr";
        public int DialogStepId { get; } = 1;
        public int DialogBranchId { get; } = 0;

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IClientService _userService;
        public S1_FirstNameInputStep(IMessageTranslationManger messageTranslationManger, IClientService userService, IBotMessageManager botMessageSender)
        {
            _messageTranslationManger = messageTranslationManger;
            _userService = userService;
            _botMessageSender = botMessageSender;
        }

        public Dictionary<Type, string> NextStepTextTag { get; } = new Dictionary<Type, string> { { typeof(S2_SurnameInputStep), null } };

        public async Task<List<string>> GetButtons(CurrentDialog currentDialog, string templatePath)
        {
            var buttns = await StepService.AddBaseButtons(_messageTranslationManger);
            return buttns;
        }
        
        public async Task SendReplyToUser(CurrentDialog currentDialog, string translationCollectionName)
        {
            var message = await GetMessage(currentDialog, translationCollectionName);
            var btns = await GetButtons(currentDialog, translationCollectionName);
            var markup = BotReplyKeyboardMarkupHandler.FormReplyKeyboardMarkupFromButtons(btns);
            await _botMessageSender.Send(message, markup);
        }

        public Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var buttonTypeAnswers = new ButtonTypeAnswer { Answer = clientAnswer };

            if (!AnswerTypeHandling.AnswerTypeGuard.AnswerIsCorrect(AnswerType.Name, buttonTypeAnswers))
            {
                var errorMessage = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _errorMessage);
                await _botMessageSender.Send(errorMessage);
                return currentDialog;
            }
            currentDialog.Client.FirstName = clientAnswer;

            currentDialog.CurrentStepType = NextStepTextTag.FirstOrDefault(s => s.Value == null).Key.ToString();
            return currentDialog;
        }

        public int TemplateId { get; } = 1;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 1;
        
        public async Task<string> GetMessage(CurrentDialog currentDialog, string translationCollectionName)
        {
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _messageTag);
            return message;
        }
    }
}
