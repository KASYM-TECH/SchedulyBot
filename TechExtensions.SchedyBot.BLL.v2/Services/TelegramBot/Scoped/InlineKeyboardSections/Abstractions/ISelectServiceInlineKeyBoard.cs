using TechExtensions.SchedyBot.DLL.Entities;
using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections.Abstractions
{
    public interface ISelectServiceInlineKeyBoard
    {
        public delegate Task HandleMethod(CurrentDialog currentDialog, CallbackQuery callbackQuery);
        public HandleMethod handleShosenService { get; set; }
        public HandleMethod handleKeyboardExit { get; set; }
        public Task Launch(CurrentDialog currentDialog, bool onlyServicesOfExecutor = false);
        public Task<CurrentDialog> Handle(CurrentDialog currentDialog, Update update, bool onlyServicesOfExecutor = false);

    }
}
