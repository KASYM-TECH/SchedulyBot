using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechExtensions.SchedyBot.BLL.TelegramBotLinkModule.Abstractions
{
    public interface ITelegramBotLinkManager
    {
        /// <summary>
        /// Генерирует ссылку на бота
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public Task<string> GenerateLink(long chatId);
        /// <summary>
        /// Возращает значение по ключу
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public long GetChatIdByLink(string link);

        public static string BasisForLink { get; }
        public bool KeyExists(string link);


    }
}
