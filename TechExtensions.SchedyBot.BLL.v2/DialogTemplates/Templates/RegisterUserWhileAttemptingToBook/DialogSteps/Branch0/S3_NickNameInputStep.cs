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
    public class S3_NickNameInputStep : IDialogStep
    {
        private string _messageTag = "branch0step3mess";
        private string _errorMessage = "branch0NickNEnterErr";

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IClientService _userService;

        public Dictionary<Type, string> NextStepTextTag { get; } 
        public S3_NickNameInputStep(IMessageTranslationManger messageTranslationManger, IClientService userService, IBotMessageManager botMessageSender)
        {
            _userService = userService;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
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
            var isSyntaxCorrect = AnswerTypeHandling.AnswerTypeGuard.AnswerIsCorrect<string>(AnswerType.Nickname, clientAnswer);
            var isInDbAnySameNicknames = _userService.GetUser(u => u.Nickname == clientAnswer) is not null;

            if ((!isSyntaxCorrect) || isInDbAnySameNicknames)
            {
                var errorMessage = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _errorMessage);
                await _botMessageSender.Send(errorMessage);
                return currentDialog;
            }
            currentDialog.Client.Nickname = clientAnswer;
            currentDialog.CurrentStepType = typeof(S4_NowYouCanStartBookingStep).ToString();
            return currentDialog;
        }

        public int TemplateId { get; } = 1;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 3;
        
        public async Task<List<string>> GetButtons(CurrentDialog currentDialog, string templatePath)
        {
            var buttons = await StepService.AddBaseButtons(_messageTranslationManger);
            return buttons;
        }

        public async Task<string> GetMessage(CurrentDialog currentDialog, string translationCollectionName)
        {
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _messageTag);

            return message;
        }
    }
}
