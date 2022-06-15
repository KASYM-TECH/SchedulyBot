using System;

namespace TechExtensions.SchedyBot.DLL.Entities
{
    public class ServiceAndSpec : BaseEntity
    {
        public Client User { get; set; }
        public Service Service { get; set; }
        public int Price { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
