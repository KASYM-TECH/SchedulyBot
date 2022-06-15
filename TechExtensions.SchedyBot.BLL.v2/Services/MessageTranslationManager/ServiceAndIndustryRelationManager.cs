using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions;

namespace TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager
{
    public class ServiceAndIndustryRelationManager : IServiceAndIndustryRelationManager
    {
        private readonly string _pathToDialogTextTranslation;
        private readonly ILogger<ServiceAndIndustryRelationManager> _logger;
        public ServiceAndIndustryRelationManager(IConfiguration configuration, ILogger<ServiceAndIndustryRelationManager> logger)
        {
            _logger = logger;
            _pathToDialogTextTranslation = configuration["PathToIndustryAndServiceTagsRelation"];
        }
        public async Task<List<string>> GetAllServiceTagsByIndustryTag(string industryTag)
        {
            await using var stream = new FileStream(
                _pathToDialogTextTranslation,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);
            try
            {
                using var sr = new StreamReader(stream, Encoding.UTF8);
                var myJsonString = await sr.ReadToEndAsync();
                var jsn = JObject.Parse(myJsonString).SelectToken("TagsRelation");

                var cool = (jsn[industryTag] as JArray).ToList().Select(a => a.ToString()).ToList();
                return cool;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Код индустрии: {industryTag}");
            }
            return null;
        }
        public async Task<string> GetIndustryTagByServiceTag(string serviceTag)
        {
            await using var stream = new FileStream(
                _pathToDialogTextTranslation,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);
            try
            {
                using var sr = new StreamReader(stream, Encoding.UTF8);
                var myJsonString = await sr.ReadToEndAsync();
                var serviceIndustry = JObject
                    .Parse(myJsonString).SelectToken("TagsRelation")
                    .FirstOrDefault(j => j.First().Any(g => g.ToString() == serviceTag)) as JProperty;

                return serviceIndustry?.Name;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Код индустрии: {serviceTag}");
            }
            return null;
        }
    }
}
