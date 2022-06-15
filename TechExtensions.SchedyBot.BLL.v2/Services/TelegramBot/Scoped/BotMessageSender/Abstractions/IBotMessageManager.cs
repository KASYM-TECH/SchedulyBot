using Telegram.Bot.Types.ReplyMarkups;

namespace TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.BotMessageSender.Abstractions
{

    public interface IBotMessageManager
    {
        /// <summary>
        /// Отправляет сообщение телеграм пользователю
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="message"></param>
        /// <param name="replyKeyboardMarkup"></param>
        /// <returns></returns>
        Task Send(string message, IReplyMarkup replyKeyboardMarkup = null, long chatId = 0);
        /// <summary>
        /// Уберает кнопки 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="message"></param>
        /// <param name="replyKeyboardMarkup"></param>
        /// <returns></returns>
        public Task RemoveButtons(string message);

        /// <summary>
        /// Отправляет начальные кнопки 
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public Task ReturnMainButtonsToClient();

        /// <summary>
        /// Обновляет InlineKeyboard
        /// </summary>
        /// <param name="CallbackQueryId"></param>
        /// <param name="chatId"></param>
        /// <param name="message"></param>
        /// <param name="inlineKeyboardMarkup"></param>
        /// <returns></returns>
        public Task EditInlineKeyboard(int CallbackQueryId, string message, InlineKeyboardMarkup inlineKeyboardMarkup);

        /// <summary>
        /// Удаляет InlineKeyboard
        /// </summary>
        /// <param name="CallbackQueryId"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public Task DeleteInlineKeyboard(int CallbackQueryId);

    }
}
