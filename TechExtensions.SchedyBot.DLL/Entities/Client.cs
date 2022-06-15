using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechExtensions.SchedyBot.DLL.Entities
{
    public class Client : BaseEntity
    {
        public string FirstName { get; set; }
        public long ChatId { get; set; }
        public Address Address { get; set; }
        public string Language { get; set; }
        public List<Schedule> Schedules { get; set; }
        public TimeSpan TimeZoneOffset { get; set; }
        public string Nickname { get; set; }
        public List<ActionHistory> ActionHistory { get; set; }
        public Contact Contact { get; set; }
        public string LastName { get; set; }
        public bool IsSeller { get; set; }
        public bool IsAdmin { get; set; } = false;
        public List<int> UsedSellerIds { get; set; }
        public bool WentThroughFullRegistration { get; set; } = false;
        public Client()
        {
            UsedSellerIds = new List<int>();
            Schedules = new List<Schedule>();
        }
        public string GetFullName()
        {
            var fullName = FirstName + " " + LastName;
            return fullName;
        }
    }
}
