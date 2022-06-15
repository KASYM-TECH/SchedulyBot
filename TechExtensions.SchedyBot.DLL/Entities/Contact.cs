using System.ComponentModel.DataAnnotations;

namespace TechExtensions.SchedyBot.DLL.Entities
{
    public class Contact : BaseEntity
    {
        [Phone]
        public string PhoneNumber { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
