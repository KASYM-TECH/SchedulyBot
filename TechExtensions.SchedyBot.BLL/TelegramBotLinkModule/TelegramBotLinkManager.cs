using Microsoft.Extensions.Caching.Memory;
using RandomStringCreator;
using System;
using System.Threading.Tasks;
using TechExtensions.SchedyBot.BLL.TelegramBotLinkModule.Abstractions;


namespace TechExtensions.SchedyBot.BLL.TelegramBotLinkModule
{
    public class TelegramBotLinkManager : ITelegramBotLinkManager
    {
        public static string BasisForLink { get;  } = "https://t.me/SchedulyBot?start=";
        private readonly IMemoryCache _memoryCache;
        public TelegramBotLinkManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<string> GenerateLink(long chatId)
        {
            var generatedString = new StringCreator().Get(10);
            var cachedValue = await _memoryCache.GetOrCreateAsync(
                    generatedString,
                    cacheEntry =>
                    {
                        cacheEntry.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10);
                        cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(10);
                        return Task.FromResult(chatId);
                    });
            return BasisForLink + generatedString;
        }
        public bool KeyExists(string link)
        {
            if (link.Contains(" ") is false)
                return false;
            var key = link.Split(' ')[1];
            var keyExists = _memoryCache.TryGetValue<long>(key, out long cachedValue);
            return keyExists;
        }
        public long GetChatIdByLink(string link)
        {
            var key = link.Split(' ')[1];
            var keyExists = _memoryCache.TryGetValue<long>(key, out long cachedValue);
            return cachedValue;
        }
    }
}
