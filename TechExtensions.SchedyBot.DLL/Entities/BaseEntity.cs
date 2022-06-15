using System;
using System.ComponentModel.DataAnnotations;

namespace TechExtensions.SchedyBot.DLL.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    }
}
