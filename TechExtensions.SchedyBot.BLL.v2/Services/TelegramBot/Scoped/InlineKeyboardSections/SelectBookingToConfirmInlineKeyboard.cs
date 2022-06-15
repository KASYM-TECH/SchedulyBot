using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;
using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections
{
    public class SelectBookingToConfirmInlineKeyboard : ISelectBookingToConfirmInlineKeyboard
    {
        // private readonly IBotMessageManager _botMessageManager;
        // private readonly IMessageTranslationManger _messageTranslationManger;
        // private readonly IReservationManager _reservationManager;
        // private readonly IScheduleService _scheduleService;
        // private readonly IBookingInConfirmationService _bookingInConfirmationService;    
        // private readonly IBookingService _bookingService;
        // public ISelectBookingToConfirmInlineKeyboard.HandleMethod handleKeyboardExit { get; set; }
        // public SelectBookingToConfirmInlineKeyboard(IBotMessageManager botMessageManager,
        //     IReservationManager reservationManager,
        //     IBookingInConfirmationService bookingInConfirmationService,
        //     IBookingService bookingService,
        //     IScheduleService scheduleService,
        //     IMessageTranslationManger messageTranslationManger)
        // {
        //     _scheduleService = scheduleService;
        //     _bookingInConfirmationService = bookingInConfirmationService;
        //     _bookingService = bookingService;
        //     _botMessageManager = botMessageManager;
        //     _reservationManager = reservationManager;
        //     _messageTranslationManger = messageTranslationManger;
        // }
        //
        // public async Task Launch(CurrentDialog currentDialog)
        // {
        //     var markup = await GetMarkup(currentDialog);
        //     var message = await _messageTranslationManger
        //         .GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.SelectBookingToConfirm);
        //     await _botMessageManager.SendInlineKeyboard(message, markup);
        //     currentDialog.CurrentInlineKeyboard = typeof(SelectBookingToConfirmInlineKeyboard)
        //         .ToString().Split(".").Last();
        //     currentDialog.State = CurrentDialogState.BeingUsedByInlineKeyboard;
        //
        // }
        //
        // private async Task<InlineKeyboardMarkup> GetMarkup(CurrentDialog currentDialog)
        // {
        //     var allUnconfirmedBookings = _bookingService.GetBookings(b => (b.Status == BookingCompletionStatus.AwaitingConfirmation 
        //     || b.Status == BookingCompletionStatus.IsInConfirmingTemplate)
        //     && b.Executor.Id == currentDialog.Client.Id);
        //     var buttons = new List<List<InlineKeyboardButton>>();
        //     foreach (var booking in allUnconfirmedBookings)
        //         buttons.Add(new List<InlineKeyboardButton>()
        //         {
        //             new InlineKeyboardButton(booking.Client.FirstName + " " + booking.Date.ToShortDateString())
        //             {
        //                 CallbackData = booking.Id.ToString()
        //             }
        //         });
        //
        //     var markup = new InlineKeyboardMarkup(buttons);
        //
        //     return markup;
        // }
        //
        // public async Task<CurrentDialog> Handle(CurrentDialog currentDialog, Update update)
        // {
        //     await _botMessageManager.DeleteInlineKeyboard(update.CallbackQuery.Message.MessageId);
        //     var selectedBooking = _bookingService.GetBooking(b => b.Id.ToString() == update.CallbackQuery!.Data);
        //     selectedBooking.Status = BookingCompletionStatus.IsInConfirmingTemplate;
        //     currentDialog.CurrentInlineKeyboard = null;
        //     currentDialog.State = CurrentDialogState.BeingUsedByInlineKeyboard;
        //     await _bookingService.Update(selectedBooking);
        //     var newBookingInConfirmation = new BookingInConfirmation() { BookingToConfirm = selectedBooking };
        //     await _bookingInConfirmationService.Create(newBookingInConfirmation);
        //     return currentDialog;
        //
        // }
        public ISelectBookingToConfirmInlineKeyboard.HandleMethod handleKeyboardExit { get; set; }
        public Task Launch(CurrentDialog currentDialog)
        {
            throw new NotImplementedException();
        }

        public Task<CurrentDialog> Handle(CurrentDialog currentDialog, Update update)
        {
            throw new NotImplementedException();
        }
    }
}
