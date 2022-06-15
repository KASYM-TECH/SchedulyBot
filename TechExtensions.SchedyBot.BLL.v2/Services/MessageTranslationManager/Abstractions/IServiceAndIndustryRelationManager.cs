namespace TechExtensions.SchedyBot.BLL.v2.Services.MessageTranslationManager.Abstractions
{
    public interface IServiceAndIndustryRelationManager
    {
        public Task<List<string>> GetAllServiceTagsByIndustryTag(string industryTag);
        public Task<string> GetIndustryTagByServiceTag(string serviceTag);

    }
}
