namespace TechExtensions.SchedyBot.DLL.Entities
{
    public class ActionHistory : BaseEntity
    {
        //public Client ObjectClient { get; set; }
        public Client SubjectClient { get; set; }
        public  string Action { get; set; }
        //public ActionType Action { get; set; }
    }
}
