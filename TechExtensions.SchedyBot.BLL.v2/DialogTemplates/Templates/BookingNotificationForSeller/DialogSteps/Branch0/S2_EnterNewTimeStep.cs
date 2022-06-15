using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling.Models;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingNotificationForSeller.DialogSteps.Branch1;
using TechExtensions.SchedyBot.BLL.v2.Services.EntityServices.DI;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions;
using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Statics;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Templates.BookingNotificationForSeller.DialogSteps.Branch0
{
    public class S2_EnterNewTimeStep : IDialogStep 
    {
        private string _messageTag = "branch0step2mess";
        private string _wrongFormatErrorMessage = "branch0step2errmess";

        public Dictionary<Type, string> NextStepTextTag { get; } = new Dictionary<Type, string>
        {
            {typeof(S1_WhatToDoWithSelectedBookingStep), "branch0step0errmess" }
        };

        private readonly IMessageTranslationManger _messageTranslationManger;
        private readonly IBotMessageManager _botMessageSender;
        private readonly IBookingInConfirmationService _bookingInConfirmationService;
        private readonly IBookingService _bookingService;
        private readonly IReservationManager _reservationManager;
        public S2_EnterNewTimeStep(IMessageTranslationManger messageTranslationManger,
             IBotMessageManager botMessageSender,
             IBookingService bookingService,
             IBookingInConfirmationService bookingInConfirmationService,
             IReservationManager reservationManager)
        {
            _bookingInConfirmationService = bookingInConfirmationService;
            _bookingService = bookingService;
            _reservationManager = reservationManager;
            _messageTranslationManger = messageTranslationManger;
            _botMessageSender = botMessageSender;
        }

        public async Task SendReplyToUser(CurrentDialog currentDialog, string translationCollectionName)
        {
            var message = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _messageTag);
            var btns = await GetButtons(currentDialog, translationCollectionName);
            var markup = BotReplyKeyboardMarkupHandler.FormReplyKeyboardMarkupFromButtons(btns);
            await _botMessageSender.Send(message, markup);
        }

        public Task<DialogIteration?> HandleAnswerAndGetNextIteration(string clientAnswer)
        {
            var isSyntaxCorrect = AnswerTypeHandling.AnswerTypeGuard.AnswerIsCorrect<string>(AnswerType.TimeSpan, clientAnswer);

            if (!isSyntaxCorrect)
            {
                var errorMessage = await _messageTranslationManger.GetTranslatedTextByTag(translationCollectionName, _wrongFormatErrorMessage);
                await _botMessageSender.Send(errorMessage);
                return currentDialog;
            }
            var bookingUnderConsideration = _bookingInConfirmationService
                .Get(b => b.BookingToConfirm.Executor.Id == currentDialog.Client.Id && b.BookingToConfirm.Status
                == BookingCompletionStatus.IsInConfirmingTemplate);
            
            var t1h = Int32.Parse(clientAnswer.Split('-')[0].Split(':')[0]);
            var t1m = Int32.Parse(clientAnswer.Split('-')[0].Split(':')[1]);
            var t2h = Int32.Parse(clientAnswer.Split('-')[1].Split(':')[0]);
            var t2m = Int32.Parse(clientAnswer.Split('-')[1].Split(':')[1]);
            bookingUnderConsideration!.TimeIsBeingChangedFrom = new DateTime(bookingUnderConsideration.BookingToConfirm.BookTimeFrom.Year,
               bookingUnderConsideration.BookingToConfirm.BookTimeFrom.Month, bookingUnderConsideration.BookingToConfirm.BookTimeFrom.Day, t1h, t1m, 0);
            bookingUnderConsideration.TimeIsBeingChangedTo = new DateTime(bookingUnderConsideration.BookingToConfirm.BookTimeTo.Year,
               bookingUnderConsideration.BookingToConfirm.BookTimeTo.Month, bookingUnderConsideration.BookingToConfirm.BookTimeTo.Day, t2h, t2m, 0);
            
            var isBookingTimeFree = await _reservationManager.TryToConfirmBookingTime(bookingUnderConsideration.BookingToConfirm);
            if (isBookingTimeFree)
                currentDialog.CurrentStepType = typeof(S3_ConfirmChosenTimeStep).ToString();
            else
                currentDialog.CurrentStepType = typeof(S0_OverlapIsDetectedStep).ToString();
            await _bookingInConfirmationService.Update(bookingUnderConsideration);
            return currentDialog; 
        }

        public int TemplateId { get; } = 3;
        public int BranchId { get; } = 0;
        public int StepId { get; } = 2;
        
        private async Task<List<string>> GetButtons(CurrentDialog currentDialog, string translationCollectionName)
        {
            var buttns = await StepService.AddBaseButtons(_messageTranslationManger);
            return buttns;
        }
    }
}
