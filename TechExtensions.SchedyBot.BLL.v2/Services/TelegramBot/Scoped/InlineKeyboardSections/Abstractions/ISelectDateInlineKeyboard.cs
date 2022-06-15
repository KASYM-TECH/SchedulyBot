using TechExtensions.SchedyBot.DLL.Entities;
using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.InlineKeyboardSections.Abstractions
{
    public interface ISelectDateInlineKeyboard
    {
        public delegate Task HandleMethod(CurrentDialog currentDialog, CallbackQuery callbackQuery);
        public CurrentDialog CurrentDialog { get; set; }
        public HandleMethod handleKeyboardExit { get; set; }
        public HandleMethod handleDate { get; set; }
        /// <summary>
        /// Запускает InlineKeyBoardSection
        /// </summary>
        /// <param name="currentDialog"></param>
        /// <returns></returns>
        public Task Launch(CurrentDialog currentDialog);

        /// <summary>
        /// Обновляет InlineKeyBoardSection
        /// </summary>
        /// <param name="currentDialog"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public Task<CurrentDialog> Handle(CurrentDialog currentDialog, Update update);
    }
}
