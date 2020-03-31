using System;

namespace Toggl.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime BeginningOfWeek(this DateTime time, BeginningOfWeek beginningOfWeek)
        {
            var dateTimeOffset = new DateTimeOffset(time, TimeSpan.Zero);
            return dateTimeOffset.BeginningOfWeek(beginningOfWeek).Date;
        }
    }
}
