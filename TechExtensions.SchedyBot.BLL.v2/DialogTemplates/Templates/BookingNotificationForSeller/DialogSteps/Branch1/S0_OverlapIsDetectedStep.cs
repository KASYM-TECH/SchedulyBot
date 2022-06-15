using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingNotificationForSeller.DialogSteps.Branch0;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingNotificationForSeller.DialogSteps.Branch1
{
    public class S0_OverlapIsDetectedStep : IDialogStep
    {
        private string _messageTag = "branch1step0mess";
        private string _errorMessage = TextTag.BtnErr;

        public Dictionary<Type, string> NextStepTextTag { get; } = new Dictionary<Type, string>
        {
            {typeof(S2_EnterNewTimeStep), S3_ConfirmChosenTimeStep.ChangeTimeBtn },
            {typeof(S4_WantToPinMessageStep), S3_ConfirmChosenTimeStep.YesAllIsCalculatedBtn }
        };
        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IBookingInConfirmationService _bookingInConfirmationService;
        private readonly IBookingService _bookingService;
        public S0_OverlapIsDetectedStep(IMessageTranslationManger messageTranslationManger,
            IBookingService bookingService,
            IBookingInConfirmationService bookingInConfirmationService,
            IBotMessageManager botMessageManager)
        {
            _botMessageSender = botMessageManager;
            _bookingInConfirmationService = bookingInConfirmationService;
            _bookingService = bookingService;
            _messageTranslationManger = messageTranslationManger;
        }

        public async Task SendReplyToUser(CurrentDialog currentDialog, string translationCollectionName)
        {
            var bookingUnderConsideration = _bookingService
                 .GetBooking(b => b.Executor.Id == currentDialog.Client.Id && b.Status
                 == BookingCompletionStatus.IsInConfirmingTemplate);
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _messageTag);
            
            var btns = await GetButtons(currentDialog, translationCollectionName);
            var markup = BotReplyKeyboardMarkupHandler.FormReplyKeyboardMarkupFromButtons(btns);
            await _botMessageSender.Send(message, markup);
        }

        public Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var buttons = await GetButtons(currentDialog, translationCollectionName);
            var buttonTypeAnswers = new ButtonTypeAnswer { Answer = clientAnswer, AnswersCollection = buttons.ToArray() };

            if (!AnswerTypeHandling.AnswerTypeGuard.AnswerIsCorrect<ButtonTypeAnswer>(AnswerType.Name, buttonTypeAnswers))
            {
                var errorMessage = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _errorMessage);
                await _botMessageSender.Send(errorMessage);
                return currentDialog;
            }
            var answerTag = await _messageTranslationManger.GetTagByText(translationCollectionName, clientAnswer);
            if(answerTag == S3_ConfirmChosenTimeStep.YesAllIsCalculatedBtn)
            {
                var bookingUnderSellerConsideration = _bookingInConfirmationService.Get(b => b.BookingToConfirm.Status == BookingCompletionStatus.IsInConfirmingTemplate
                    && b.BookingToConfirm.Executor.Id == currentDialog.Client.Id);
                bookingUnderSellerConsideration!.Status = BookingCompletionStatusDuringSellerConfirmation.TimeIsBeingChanged;
                await _bookingInConfirmationService.Update(bookingUnderSellerConsideration);
            }
            currentDialog.CurrentStepType = NextStepTextTag.FirstOrDefault(n => n.Value == answerTag).Key.ToString();
            return currentDialog;
        }

        public int TemplateId { get; } = 3;
        public int BranchId { get; } = 1;
        public int StepId { get; } = 0;
        
        private async Task<List<string>> GetButtons(CurrentDialog currentDialog, string translationCollectionName)
        {
            var buttons = await StepService.AddBaseButtons(_messageTranslationManger);
            foreach (var tag in NextStepTextTag)
            {
                var translatedBtn = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, tag.Value);
                buttons.Add(translatedBtn);
            }
            return buttons;
        }

    }
}
