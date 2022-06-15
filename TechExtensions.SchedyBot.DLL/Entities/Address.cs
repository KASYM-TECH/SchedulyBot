namespace TechExtensions.SchedyBot.DLL.Entities
{
    public class Address : BaseEntity
    {
        //Понадобится для интеграции перевода в архитектуру 
        public Country Country { get; set; }
        //Для клиентов, чтобы знали где продавец находится 
        public string City { get; set; }
        //По желанию, но клиену может понадобится 
        public string FullAddress { get; set; }
    }
    //Понадобится для интеграции перевода в архитектуру ?
    public enum Country 
    {
        Russia,
        USA
    }

}
