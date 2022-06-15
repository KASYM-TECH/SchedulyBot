using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.BotDialogProcessing.Containers.Abstractions
{
    public interface IUpdateContainer
    {
        public Update Update { get; set; }
    }
}
