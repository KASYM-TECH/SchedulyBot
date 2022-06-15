using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.BLL.v2.Extensions;

public static class DateTimeExtensions
{
    public static WeekDay ToWeekDay(this DayOfWeek dayOfWeek)
    {
        var weekDay = (WeekDay) dayOfWeek;
        if (dayOfWeek == 0)
            weekDay = WeekDay.Sunday;

        return weekDay;
    }
}