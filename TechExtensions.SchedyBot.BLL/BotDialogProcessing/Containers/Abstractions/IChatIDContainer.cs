using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.BotDialogProcessing.Containers.Abstractions
{
    public interface IChatIDContainer
    {
        public void SetChatId(Update update);
        public long ChatId { get; set; }
    }
}
