namespace TechExtensions.SchedyBot.DLL.Entities
{
    public class UpdateMessage : BaseEntity
    {
        // не менять значения MessageId и ChatId после первичного присваивания, иначе HashCode будет плохо
        public int MessageId { get; set; }
        public string ChatId { get; set; }
        public int HashCode { get; set; }
        public override int GetHashCode()
        {
            return (MessageId, ChatId).GetHashCode();
        } 
    }
}
