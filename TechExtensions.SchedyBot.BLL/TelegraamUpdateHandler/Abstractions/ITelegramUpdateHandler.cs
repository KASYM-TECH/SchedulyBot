using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.TelegraamUpdateHandler.Abstractions
{
    public interface ITelegramUpdateHandler
    {
        /// <summary>
        /// Обрабатывет сообщение от телеграм пользователя
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        Task Handle(Update update);

    }
}
