using System;

namespace Toggl.Shared.Extensions
{
    public static class CalendarMonthExtensions
    {
        public static DateTime ToDateTime(this CalendarMonth calendarMonth)
            => new DateTime(calendarMonth.Year, calendarMonth.Month, 1);
    }
}
