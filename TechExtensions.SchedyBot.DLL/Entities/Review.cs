namespace TechExtensions.SchedyBot.DLL.Entities
{
    public class Review : BaseEntity
    {
        public Client ReviewUser { get; set; }
        public Client TargetUser { get; set; }
        public string ReviewText { get; set; }
        public int Rate { get; set; }

    }
}
