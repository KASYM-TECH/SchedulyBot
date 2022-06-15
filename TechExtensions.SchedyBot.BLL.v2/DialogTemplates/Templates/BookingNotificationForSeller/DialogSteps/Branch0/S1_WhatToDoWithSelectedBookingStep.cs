using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling.Models;
using TechExtensions.SchedyBot.BLL.v2.Models.Constants;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingNotificationForSeller.DialogSteps.Branch0
{
    public class S1_WhatToDoWithSelectedBookingStep : IDialogStep
    {
        private string _messageTag = "branch0step1mess";
        private string _errorMessage = TextTag.BtnErr;

        public Dictionary<string, Type> NextStepTextTag { get; } = new Dictionary<string, Type>
        {
            {TextTag.Confirm, typeof(S4_WantToPinMessageStep) },
            {TextTag.RejectBtn,  typeof(S4_WantToPinMessageStep)},
            {TextTag.ChangeTime,  typeof(S2_EnterNewTimeStep)}
        };
        private readonly ISelectBookingToConfirmInlineKeyboard _selectBookingToConfirmInlineKeyboard;
        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IServiceAndSpecService _serviceAndSpecService;
        private readonly IServiceKindService _serviceKindService;
        private readonly IServiceService _serviceService;
        private readonly IBookingInConfirmationService _bookingInConfirmationService;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IBookingService _bookingService;
        public S1_WhatToDoWithSelectedBookingStep(ISelectBookingToConfirmInlineKeyboard selectBookingToConfirmInlineKeyboard,
            IMessageTranslationManger messageTranslationManger,
            IServiceAndSpecService serviceAndSpecService,
            IServiceKindService serviceKindService,
            IServiceService serviceService,
            IBookingInConfirmationService bookingInConfirmationService,
            IBookingService bookingService, 
            IBotMessageManager botMessageSender)
        {
            _bookingInConfirmationService = bookingInConfirmationService;
            _bookingService = bookingService;
            _serviceService = serviceService;
            _botMessageSender = botMessageSender;
            _serviceAndSpecService = serviceAndSpecService;
            _serviceKindService = serviceKindService;
            _messageTranslationManger = messageTranslationManger;
            _selectBookingToConfirmInlineKeyboard = selectBookingToConfirmInlineKeyboard;
        }

        public async Task SendReplyToUser(CurrentDialog currentDialog, string translationCollectionName)
        {
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _messageTag);
            var s1 = _serviceAndSpecService.GetManyServiceAndSpec(s => s.IsActive);
            var s2 = _serviceKindService.GetServices(s => s.IsActive);
            var s3 = _serviceService.GetServices(s => s.IsActive);
            var bookingUnderSellerConsideration = _bookingInConfirmationService.Get(b => b.BookingToConfirm.Status == BookingCompletionStatus.IsInConfirmingTemplate
            && b.BookingToConfirm.Executor.Id == currentDialog.Client.Id);
            var replacedMessage = message
                .Replace("{user.name}", bookingUnderSellerConsideration!.BookingToConfirm.Client.FirstName)
                .Replace("{date}", bookingUnderSellerConsideration.BookingToConfirm.Date.ToShortDateString())
                .Replace("{from}", bookingUnderSellerConsideration.BookingToConfirm.BookTimeFrom.ToShortTimeString())
                .Replace("{to}", bookingUnderSellerConsideration.BookingToConfirm.BookTimeTo.ToShortTimeString());
            var buttons = await StepService.AddBaseButtons(_messageTranslationManger);
            var btnTags = NextStepTextTag.Select(n => n.Key);
            foreach (var btnTag in btnTags)
                buttons.Add(await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, btnTag));
            var markup = BotReplyKeyboardMarkupHandler.FormReplyKeyboardMarkupFromButtons(buttons);
            await _botMessageSender.Send(replacedMessage, markup);
        }

        public Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var allTranslatedBtns = new List<string>();
            foreach (var tag in NextStepTextTag)
            {
                var translatedBtn = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, tag.Key);
                allTranslatedBtns.Add(translatedBtn);
            }
            var buttonTypeAnswers = new ButtonTypeAnswer { Answer = clientAnswer, AnswersCollection = allTranslatedBtns.ToArray() };

            if (!AnswerTypeHandling.AnswerTypeGuard.AnswerIsCorrect<ButtonTypeAnswer>(AnswerType.Name, buttonTypeAnswers))
            {
                var errorMessage = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, _errorMessage);
                await _botMessageSender.Send(errorMessage);
                return currentDialog;
            }
            
            var bookingUnderSellerConsideration = _bookingInConfirmationService.Get(b => b.BookingToConfirm.Status == BookingCompletionStatus.IsInConfirmingTemplate
                    && b.BookingToConfirm.Executor.Id == currentDialog.Client.Id);
            var answerTag = await _messageTranslationManger.GetTagByText(CollectionName.CommonTranslations, clientAnswer);
            if(answerTag == TextTag.Confirm)
                bookingUnderSellerConsideration!.Status = BookingCompletionStatusDuringSellerConfirmation.IsBeingConfirmed;
            else if(answerTag == TextTag.RejectBtn)
                bookingUnderSellerConsideration!.Status = BookingCompletionStatusDuringSellerConfirmation.IsBeingCanceled;
            else if(answerTag == TextTag.ChangeTime)
                bookingUnderSellerConsideration!.Status = BookingCompletionStatusDuringSellerConfirmation.TimeIsBeingChanged;

            await _bookingInConfirmationService.Update(bookingUnderSellerConsideration!);
            var stepProperty = NextStepTextTag.FirstOrDefault(n => n.Key == answerTag);
            currentDialog.CurrentStepType = stepProperty.Value.ToString();
            return currentDialog;
        }

        public int TemplateId { get; } = 3;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 1;
    }
}
