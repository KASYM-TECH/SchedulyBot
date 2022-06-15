using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechExtensions.SchedyBot.DLL.Entities
{
    public class Schedule : BaseEntity
    {
        public int ClientId { get; set; }
        public WeekDay WeekDay { get; set; }
        public DateTime BreakTimeFrom { get; set; }
        public DateTime BreakTimeTo { get; set; }
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
    }

    public enum WeekDay
    {
        [Description("Понедельник")]
        Monday = 1,
        [Description("Вторник")]
        Tuesday = 2,
        [Description("Среда")]
        Wednesday = 3,
        [Description("Четверг")]
        Thursday = 4,
        [Description("Пятница")]
        Friday = 5,
        [Description("Суббота")]
        Saturday = 6,
        [Description("Воскресенье")]
        Sunday = 7
    }
}
