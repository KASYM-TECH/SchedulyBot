using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;
using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections
{
    public class ListAllBookingsOfUserInlineKeyboard : IListAllBookingsOfUserInlineKeyboard
    {
        // private readonly IBotMessageSender _botMessageSender;
        // private readonly IMessageTranslationManger _messageTranslationManger;
        // // private readonly IReservationManager _reservationManager;
        // // private readonly IScheduleService _scheduleService;
        // // private readonly IBookingService _bookingService;
        // public IListAllBookingsOfUserInlineKeyboard.HandleMethod HandleKeyboardExit { get; set; }
        // public IListAllBookingsOfUserInlineKeyboard.HandleBooking HandleChosenBooking { get; set; }
        // public ListAllBookingsOfUserInlineKeyboard(
        //     IBotMessageSender botMessageSender,
        //     // IReservationManager reservationManager,
        //     // IBookingService bookingService,
        //     // IScheduleService scheduleService,
        //     IMessageTranslationManger messageTranslationManger)
        // {
        //     // _scheduleService = scheduleService;
        //     // _bookingService = bookingService;
        //     // _reservationManager = reservationManager;
        //     _botMessageSender = botMessageSender;
        //     _messageTranslationManger = messageTranslationManger;
        // }
        //
        //
        // public async Task Launch(CurrentDialog currentDialog, WhoseBookings whoseBookings)
        // {
        //     var markup = await GetMarkup(currentDialog, whoseBookings);
        //     var message = await _messageTranslationManger
        //         .GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.SelectBookingToConfirm);
        //     await _botMessageSender.SendInlineKeyboard(message, markup);
        //     currentDialog.CurrentInlineKeyboard = typeof(ListAllBookingsOfUserInlineKeyboard)
        //         .ToString().Split(".").Last();
        //     currentDialog.State = CurrentDialogState.BeingUsedByInlineKeyboard;
        //
        // }
        //
        // private async Task<InlineKeyboardMarkup> GetMarkup(CurrentDialog currentDialog, WhoseBookings whoseBookings)
        // {
        //     var allBookings = new List<Booking>();
        //     if (whoseBookings == WhoseBookings.Executor)
        //         allBookings = _bookingService.GetBookings(b => (b.Status == BookingCompletionStatus.CanceledByClientBecauseOfTime || b.Status == BookingCompletionStatus.CanceledByClientBecauseOfTime ) && 
        //          b.Executor.Id == currentDialog.Client.Id);
        //     else
        //         allBookings = _bookingService.GetBookings(b =>( b.Status == BookingCompletionStatus.TimeIsChangedByExecutor || b.Status == BookingCompletionStatus.Confirmed || b.Status == BookingCompletionStatus.CanceledBySeller)
        //         && b.Client.Id == currentDialog.Client.Id);
        //     var buttons = new List<List<InlineKeyboardButton>>();
        //     foreach (var booking in allBookings)
        //     {
        //         var withWhomBookingIs = whoseBookings == WhoseBookings.Client ? booking.Executor : booking.Client;
        //         var bookingStatus = BookingCompletionStatus.AwaitingConfirmation.ToString();
        //         if (booking.Status == BookingCompletionStatus.Initiated)
        //             continue;
        //
        //         if (booking.Status == BookingCompletionStatus.IsInConfirmingTemplate || booking.Status == BookingCompletionStatus.UnderTelegramUserConsideration)
        //             bookingStatus = TextTag.DontTouchThisBooking;
        //         var translatedStatus = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, bookingStatus);
        //         buttons.Add(new List<InlineKeyboardButton>()
        //         {
        //             new InlineKeyboardButton(withWhomBookingIs.FirstName + " " + booking.Date.ToShortDateString() + "\n" + translatedStatus)
        //             {
        //                 CallbackData = booking.Id.ToString()
        //             }
        //         });
        //     }
        //     var cancelBtn = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.CancelBtn);
        //     buttons.Add(new List<InlineKeyboardButton>() { new InlineKeyboardButton(cancelBtn) { CallbackData = TextTag.CancelBtn } });
        //     var markup = new InlineKeyboardMarkup(buttons);
        //     return markup;
        // }
        //
        // public async Task<CurrentDialog> Handle(CurrentDialog currentDialog, Update update)
        // {
        //     if(update.CallbackQuery!.Data == TextTag.CancelBtn)
        //     {
        //         await HandleKeyboardExit(currentDialog, update.CallbackQuery);
        //         return currentDialog;
        //     }
        //     var booking = _bookingService.GetBooking(b => b.Id.ToString() == update.CallbackQuery!.Data);
        //     await HandleChosenBooking(currentDialog, booking);
        //     var messageOfBooking = "";
        //     if (booking.Executor.Id == currentDialog.Client.Id)
        //         messageOfBooking += booking.Client.FirstName + booking.Client.LastName;
        //     else
        //         messageOfBooking += booking.Executor.FirstName + booking.Executor.LastName;
        //     await _botMessageSender.DeleteInlineKeyboard(update.CallbackQuery.Message.MessageId);
        //     messageOfBooking += "\n" + booking.Date.ToShortDateString() + "\n";
        //     messageOfBooking += booking.BookTimeFrom.ToShortTimeString() + " - " + booking.BookTimeTo.ToShortTimeString();
        //     var buttons = await StepService.AddBaseButtons(_messageTranslationManger);
        //     var markup = BotReplyKeyboardMarkupHandler.FormReplyKeyboardMarkupFromButtons(buttons);
        //     await _botMessageSender.Send(messageOfBooking, markup);
        //     currentDialog.State = CurrentDialogState.BeingUsedByTemplate;
        //     await _bookingService.Update(booking);
        //     return currentDialog;
        //
        // }

        public IListAllBookingsOfUserInlineKeyboard.HandleMethod HandleKeyboardExit { get; set; }
        public IListAllBookingsOfUserInlineKeyboard.HandleBooking HandleChosenBooking { get; set; }
        public Task Launch(CurrentDialog currentDialog, WhoseBookings whoseBookings)
        {
            throw new NotImplementedException();
        }

        public Task<CurrentDialog> Handle(CurrentDialog currentDialog, Update update)
        {
            throw new NotImplementedException();
        }
    }

}
