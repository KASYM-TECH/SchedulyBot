using Microsoft.Extensions.Caching.Memory;
using RandomStringCreator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechExtensions.SchedyBot.BLL.v2.Services.TelegramBot.Scoped.TelegramBotLinkModule
{
    public class TelegramBotLinkManager
    {
        public static string BasisForLink { get; } = "https://t.me/SchedulyBot?start=";
        public static string StaticLinkCode { get; } = "FdA54f";
        private readonly IMemoryCache _memoryCache;
        public TelegramBotLinkManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<string> GenerateLink(long chatId)
        {
            var generatedString = StaticLinkCode + new StringCreator().Get(10);
            var cachedValue = await _memoryCache.GetOrCreateAsync(
                    generatedString,
                    cacheEntry =>
                    {
                        cacheEntry.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(120);
                        cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(120);
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
        public long GetIdByLink(string link)
        {
            var key = link.Split(' ')[1];
            var keyExists = _memoryCache.TryGetValue<long>(key, out long cachedValue);
            return cachedValue;
        }

        public string FormatIdToFitString(string id)
        {
            return "executorId " + id;
        }
    }
}
