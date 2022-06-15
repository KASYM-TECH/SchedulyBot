using TechExtensions.SchedyBot.BLL.BotDialogProcessing.Containers.Abstractions;
using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.BotDialogProcessing.Containers
{
    public class UpdateContainer : IUpdateContainer
    {
        public Update Update { get; set; }
    }
}
