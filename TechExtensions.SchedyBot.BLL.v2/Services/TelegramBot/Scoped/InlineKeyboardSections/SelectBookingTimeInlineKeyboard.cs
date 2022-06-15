using TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;
using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections
{
    public class SelectBookingTimeInlineKeyboard : ISelectBookingTimeInlineKeyboard
    {
        // private readonly IBotMessageManager _botMessageManager;
        // private readonly IMessageTranslationManger _messageTranslationManger;
        // private readonly IReservationManager _reservationManager;
        // private readonly IScheduleService _scheduleService;
        // private readonly IBookingService _bookingService;
        // public ISelectBookingTimeInlineKeyboard.HandleBookingTime handleDateTime { get; set; }
        // public ISelectBookingTimeInlineKeyboard.HandleMethod handleKeyboardExit { get; set; }
        // public SelectBookingTimeInlineKeyboard(IBotMessageManager botMessageManager,
        //     IReservationManager reservationManager,
        //     IBookingService bookingService,
        //     IScheduleService scheduleService,
        //     IMessageTranslationManger messageTranslationManger)
        // {
        //     _scheduleService = scheduleService;
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
        //         .GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.SelectTimeSpan);
        //     await _botMessageManager.SendInlineKeyboard(message, markup);
        //     currentDialog.CurrentInlineKeyboard = typeof(SelectBookingTimeInlineKeyboard)
        //         .ToString().Split(".").Last();
        //     currentDialog.State = CurrentDialogState.BeingUsedByInlineKeyboard;
        //
        // }
        //
        //
        // public async Task<CurrentDialog> Handle(CurrentDialog currentDialog, Update update)
        // {
        //     switch (update.CallbackQuery?.Data)
        //     {
        //         case TextTag.GetBackBtn:
        //             await handleKeyboardExit(currentDialog, update.CallbackQuery);
        //             return currentDialog;
        //         default:
        //             var bookingTimeFrom = DateTime.Parse(update.CallbackQuery.Data.Split("*")[0]);
        //             var bookingTimeTo = DateTime.Parse(update.CallbackQuery.Data.Split("*")[1]);
        //             await handleDateTime(bookingTimeFrom, bookingTimeTo, currentDialog);
        //             await _botMessageManager.DeleteInlineKeyboard(update.CallbackQuery.Message.MessageId);
        //             return currentDialog;
        //     }
        // }
        //
        // private async Task<InlineKeyboardMarkup> GetMarkup(CurrentDialog currentDialog)
        // {
        //     var initiatedBooking = _bookingService
        //         .GetBooking(b => b.Status == BookingCompletionStatus.Initiated
        //          && b.Client.Id == currentDialog.Client.Id);
        //     var scheduletInThatDay = _scheduleService.GetSchedule(s => s.ClientId == initiatedBooking.Executor.Id
        //     && initiatedBooking.Date.DayOfWeek.ToString() == s.WeekDay.ToString());
        //     //TimeZoneService.SetProperTimeZone<Schedule>(scheduletInThatDay, currentDialog.Client.TimeZoneOffset);
        //     var allFreeTimeInThatDay = await _reservationManager
        //         .GetFreeTimeScheduleByDay(scheduletInThatDay,
        //         initiatedBooking.Date,
        //         initiatedBooking.Executor.ChatId,
        //         initiatedBooking.ServiceAndSpec.Duration);
        //
        //     var buttons = new List<List<InlineKeyboardButton>>();
        //     var allFreeTimeInThatDayInChuncks = allFreeTimeInThatDay.Chunk(3);
        //     var index = 0;
        //     foreach (var timeChank in allFreeTimeInThatDayInChuncks)
        //     {
        //         buttons.Add(new List<InlineKeyboardButton>());
        //         foreach (var time in timeChank)
        //         {
        //             buttons[index].Add(new InlineKeyboardButton(time.Key.ToShortTimeString() + "-" + time.Value.ToShortTimeString())
        //             {
        //                 CallbackData = (time.Key + "*" + time.Value).ToString()
        //             }); 
        //         }
        //         index++;
        //     }
        //     var backBtn = await _messageTranslationManger.GetTranslatedTextByTag(CollectionName.CommonTranslations, TextTag.GetBackBtn);
        //     buttons.Add(new List<InlineKeyboardButton>() { new InlineKeyboardButton(backBtn) { CallbackData = TextTag.GetBackBtn } });
        //     var markup = new InlineKeyboardMarkup(buttons);
        //
        //     return markup;
        // } 
        public ISelectBookingTimeInlineKeyboard.HandleMethod handleKeyboardExit { get; set; }
        public ISelectBookingTimeInlineKeyboard.HandleBookingTime handleDateTime { get; set; }
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
