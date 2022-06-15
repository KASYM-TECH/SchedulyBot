using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.BotDialogProcessing.Containers.Abstractions
{
    public interface ILanguageInfoContainer
    {
        public void SetLanguageCode(Update update)
        {
            if (update.Message == null)
                languageCode = update.CallbackQuery.From.LanguageCode;
            else
                languageCode = update.Message.From.LanguageCode;
        }
        public string languageCode { get; set; }
    }
}
