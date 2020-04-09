using System;
using System.Globalization;
using Toggl.Core.UI.Helper;

namespace Toggl.Core.UI.Transformations
{
    public class DateTimeToFormattedString
    {
        public static string Convert(
            DateTimeOffset date,
            string format,
            TimeZoneInfo timeZoneInfo = null)
        {
            if (timeZoneInfo == null)
            {
                timeZoneInfo = TimeZoneInfo.Local;
            }

            return getDateTimeOffsetInCorrectTimeZone(date, timeZoneInfo).ToString(format, DateFormatCultureInfo.CurrentCulture);
        }

        private static DateTimeOffset getDateTimeOffsetInCorrectTimeZone(
            DateTimeOffset value,
            TimeZoneInfo timeZone)
            => value == default(DateTimeOffset) ? value : TimeZoneInfo.ConvertTime(value, timeZone);
    }
}
