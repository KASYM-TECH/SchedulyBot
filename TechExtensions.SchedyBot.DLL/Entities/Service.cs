namespace TechExtensions.SchedyBot.DLL.Entities
{
    public class Service : BaseEntity
    {
        public string Name { get; set; }
        public string LanguageCode { get; set; } = "en";
        public bool IsMain { get; set; } = false;
    }
}
