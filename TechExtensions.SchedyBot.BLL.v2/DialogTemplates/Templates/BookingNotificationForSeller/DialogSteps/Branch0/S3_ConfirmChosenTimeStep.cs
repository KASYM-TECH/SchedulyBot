using TechExtensions.SchedyBot.BLL.v2.Containers;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingNotificationForSeller.DialogSteps.Branch0
{
    public class S3_ConfirmChosenTimeStep : IDialogStep
    {
        private string _messageTag = "branch0step3mess";
        private string _errorMessage = TextTag.BtnErr;
        public static string ChangeTimeBtn = "change";
        public static string YesAllIsCalculatedBtn = "YesAllIsCalculated";

        public Dictionary<Type, string> NextStepTextTag { get; } = new Dictionary<Type, string>
        {
            {typeof(S2_EnterNewTimeStep), ChangeTimeBtn },
            {typeof(S4_WantToPinMessageStep), YesAllIsCalculatedBtn }
        };
        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IBookingService _bookingService;
        private readonly IBookingInConfirmationService _bookingInConfirmationService;
        private readonly IUpdateContainer _updateContainer;
        public S3_ConfirmChosenTimeStep(IMessageTranslationManger messageTranslationManger,
            IBookingService bookingService,
            IUpdateContainer updateContainer,
            IBookingInConfirmationService bookingInConfirmationService,
            IBotMessageManager botMessageManager)
        {
            _bookingInConfirmationService = bookingInConfirmationService;
            _updateContainer = updateContainer;
            _botMessageSender = botMessageManager;
            _bookingService = bookingService;
            _messageTranslationManger = messageTranslationManger;
        }

        public async Task SendReplyToUser(CurrentDialog currentDialog, string translationCollectionName)
        {
            var bookingUnderConsideration = _bookingInConfirmationService
                .Get(b => b.BookingToConfirm.Executor.Id == currentDialog.Client.Id && b.BookingToConfirm.Status
                == BookingCompletionStatus.IsInConfirmingTemplate);
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _messageTag);
            var replacedMessage = message.Replace("{booking.info}", bookingUnderConsideration.TimeIsBeingChangedFrom?.ToShortTimeString()
                + "-" + bookingUnderConsideration.TimeIsBeingChangedTo?.ToShortTimeString()) ;
            var btns = await GetButtons(currentDialog, translationCollectionName);
            var markup = BotReplyKeyboardMarkupHandler.FormReplyKeyboardMarkupFromButtons(btns);
            await _botMessageSender.Send(replacedMessage, markup);
        }

        public Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var buttons = await GetButtons(currentDialog, translationCollectionName);
            if (!buttons.Any(b => clientAnswer == b))
            {
                var errorMessage = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, _errorMessage);
                await _botMessageSender.Send(errorMessage);
                return currentDialog;
            }
            var bookingUnderConsideration = _bookingInConfirmationService
                .Get(b => b.BookingToConfirm.Executor.Id == currentDialog.Client.Id && b.BookingToConfirm.Status
                    == BookingCompletionStatus.IsInConfirmingTemplate);
            var answerTag = await _messageTranslationManger.GetTagByText(CollectionName.CommonTranslations, clientAnswer);
            if(answerTag == YesAllIsCalculatedBtn)
                bookingUnderConsideration!.Status = BookingCompletionStatusDuringSellerConfirmation.TimeIsBeingChanged;

            await _bookingInConfirmationService.Update(bookingUnderConsideration!);
            currentDialog.CurrentStepType = NextStepTextTag.FirstOrDefault(n => n.Value == answerTag).Key.ToString();
            return currentDialog;
        }

        public int TemplateId { get; } = 3;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 3;
        
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
