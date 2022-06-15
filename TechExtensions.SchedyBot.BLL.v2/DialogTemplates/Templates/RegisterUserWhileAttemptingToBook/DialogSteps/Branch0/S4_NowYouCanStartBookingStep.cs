using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookSellerTime.DialogSteps.Branch0;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.RegisterUserWhileAttemptingToBook.DialogSteps.Branch0
{
    public class S4_NowYouCanStartBookingStep : IDialogStep
    {
        private string _messageTag = "branch0step4mess";
        private string _errorMessage = TextTag.BtnErr;

        public Dictionary<Type, string> NextStepTextTag { get; }


        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IClientService _userService;
        public S4_NowYouCanStartBookingStep(IMessageTranslationManger messageTranslationManger, IClientService userService, IBotMessageManager botMessageSender)
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
            var answerTag = await _messageTranslationManger.GetTagByText(CollectionName.CommonTranslations, clientAnswer);
            string[] array = { TextTag.GreateBtn, TextTag.NoBtn };
            var buttonTypeAnswers = new ButtonTypeAnswer { Answer = answerTag, AnswersCollection = array };
            if (!AnswerTypeHandling.AnswerTypeGuard.AnswerIsCorrect<ButtonTypeAnswer>(AnswerType.Button, buttonTypeAnswers))
            {
                var errorMessage = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, _errorMessage);
                await _botMessageSender.Send(errorMessage);
                return currentDialog;
            }

            if(answerTag == TextTag.GreateBtn)
            {
                currentDialog.CurrentStepType = typeof(S1_ShooseServiceStep).ToString();
                currentDialog.State = CurrentDialogState.TransitionToAnotherTemplate;
            }
            else
            {
                currentDialog.CurrentStepType = null;
                currentDialog.StepTypeHistory = null;
                currentDialog.CurrentInlineKeyboard = null;
                currentDialog.State = CurrentDialogState.Suspended;
            }
            return currentDialog;
        }

        public int TemplateId { get; } = 1;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 4;
        
        public async Task<List<string>> GetButtons(CurrentDialog currentDialog, string translationCollectionName)
        {
            var buttons = await StepService.AddBaseButtons(_messageTranslationManger);

            var translatedBtn = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.GreateBtn);
            var translatedBtn1 = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.NoBtn);

            buttons.Add(translatedBtn);
            buttons.Add(translatedBtn1);

            return buttons;
        }

        public async Task<string> GetMessage(CurrentDialog currentDialog, string translationCollectionName)
        {
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _messageTag);

            return message;
        }
    }
}
