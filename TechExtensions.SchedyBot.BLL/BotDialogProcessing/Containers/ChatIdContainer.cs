using TechExtensions.SchedyBot.BLL.BotDialogProcessing.Containers.Abstractions;
using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.BotDialogProcessing.Containers
{
    public class ChatIdContainer : IChatIDContainer
    {
        public void SetChatId(Update update)
        {
            // TODO: мне не нравится, что на таком спорном решении логика построена
            if (update.Message == null)
                ChatId = update.CallbackQuery.Message.Chat.Id;
            else
                ChatId = update.Message.Chat.Id;
        }
        public long ChatId { get; set; }
    }
}
