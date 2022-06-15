using System.Threading.Tasks;
using TechExtensions.SchedyBot.BLL.Models;

namespace TechExtensions.SchedyBot.BLL.BotDialogProcessing.Abstractions
{
    public interface IUpdateManager
    {
        /// <summary>
        /// Смогу ли я как менеджер эту информацию обработать, для меня ли она?
        /// </summary>
        /// <returns></returns>
        Task<bool> DoesItForMe(UpdateModelForManage model);

        /// <summary>
        /// Начни делать что-то с данными
        /// </summary>
        /// <param name="model">Пришедшие данные</param>
        /// <returns></returns>
        Task<bool> Manage(UpdateModelForManage model);
    }
}
