using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling.Models;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingNotificationForSeller.DialogSteps.Branch0
{
    public class S4_WantToPinMessageStep : IDialogStep
    {
        private string _messageTag = "branch0step4mess";
        private string _errorMessage = TextTag.BtnErr;
        private static string NoMessageBtn = "noMessage";

        public Dictionary<Type, string> NextStepTextTag { get; } = new Dictionary<Type, string>
        {
            {typeof(S5_BookingIsConfirmedOrDeclinedStep), "noMessage" }
        };
        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IBookingService _bookingService;
        private readonly IBookingInConfirmationService _bookingInConfirmationService;

        public S4_WantToPinMessageStep(IMessageTranslationManger messageTranslationManger,
            IBookingService bookingService,
            IBookingInConfirmationService bookingInConfirmationService,
            IBotMessageManager botMessageManager)
        {
            _bookingInConfirmationService = bookingInConfirmationService;
            _botMessageSender = botMessageManager;
            _bookingService = bookingService;
            _messageTranslationManger = messageTranslationManger;
        }

        public async Task SendReplyToUser(CurrentDialog currentDialog, string translationCollectionName)
        {
            var bookingUnderConsideration = _bookingService
                 .GetBooking(b => b.Executor.Id == currentDialog.Client.Id && b.Status
                 == BookingCompletionStatus.IsInConfirmingTemplate);
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _messageTag);
            var replacesMess = message.Replace("{user.name}", bookingUnderConsideration.Client.FirstName);
            var btns = await GetButtons(currentDialog, translationCollectionName);
            var markup = BotReplyKeyboardMarkupHandler.FormReplyKeyboardMarkupFromButtons(btns);
            await _botMessageSender.Send(replacesMess, markup);
        }

        public Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var buttons = new List<string>();
            foreach (var tag in NextStepTextTag)
            {
                var translatedBtn = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, tag.Value);
                buttons.Add(translatedBtn);
            }
            var buttonTypeAnswers = new ButtonTypeAnswer { Answer = clientAnswer, AnswersCollection = buttons.ToArray() };

            if (!AnswerTypeHandling.AnswerTypeGuard.AnswerIsCorrect<ButtonTypeAnswer>(AnswerType.Name, buttonTypeAnswers))
            {
                var errorMessage = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _errorMessage);
                await _botMessageSender.Send(errorMessage);
                return currentDialog;
            }
            var answerTag = await _messageTranslationManger.GetTagByText(translationCollectionName, clientAnswer);
            var stepProperty = NextStepTextTag.First();
            var isInConfirmingTemplate = _bookingInConfirmationService.Get(b => b.BookingToConfirm.Status == BookingCompletionStatus.IsInConfirmingTemplate
                && b.BookingToConfirm.Executor.Id == currentDialog.Client.Id);
            if ((stepProperty.Value == answerTag) is false)
                isInConfirmingTemplate!.MessageForClient = clientAnswer;

            await _bookingInConfirmationService.Update(isInConfirmingTemplate!);
            currentDialog.CurrentStepType = stepProperty.Key.ToString();
            return currentDialog;
        }

        public int TemplateId { get; } = 3;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 4;
        
        private async Task<List<string>> GetButtons(CurrentDialog currentDialog, string translationCollectionName)
        {
            var buttns = await StepService.AddBaseButtons(_messageTranslationManger);
            buttns.Add(_messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, NoMessageBtn).Result);
            return buttns;
        }
    }
}
