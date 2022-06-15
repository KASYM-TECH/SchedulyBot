using Telegram.Bot.Types;

namespace TechExtensions.SchedyBot.BLL.v2.Services.UpdateHandler.DI;

public interface IUpdateHandler
{
    /// <summary>
    /// Обрабатывет сообщение от телеграм пользователя
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    Task Handle(Update update);
}