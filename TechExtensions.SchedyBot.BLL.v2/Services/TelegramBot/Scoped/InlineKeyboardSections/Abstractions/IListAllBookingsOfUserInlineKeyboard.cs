using TechExtensions.SchedyBot.DLL.Entities;
using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections.Abstractions
{
    public interface IListAllBookingsOfUserInlineKeyboard
    {
        public delegate Task HandleMethod(CurrentDialog currentDialog, CallbackQuery callbackQuery);
        public delegate Task HandleBooking(CurrentDialog currentDialog, Booking booking);
        public HandleMethod HandleKeyboardExit { get; set; }
        public HandleBooking HandleChosenBooking { get; set; }

        public Task Launch(CurrentDialog currentDialog, WhoseBookings whoseBookings);
        public Task<CurrentDialog> Handle(CurrentDialog currentDialog, Update update);

    }
    public enum WhoseBookings
    {
        Client = 0,
        Executor = 1
    }
}
