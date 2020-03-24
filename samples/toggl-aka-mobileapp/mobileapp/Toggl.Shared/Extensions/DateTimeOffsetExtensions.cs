using System;
using static Toggl.Shared.Math;
using SystemMath = System.Math;

namespace Toggl.Shared.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        private const int minAllowedYear = 2006;
        private const int maxAllowedYear = 2030;

        public static long ToUnixTimeSeconds(this DateTimeOffset dateTime)
        {
            // constant and implementation taken from .NET reference source:
            // https://referencesource.microsoft.com/#mscorlib/system/datetimeoffset.cs
            const long unixEpochSeconds = 62_135_596_800L;

            var seconds = dateTime.UtcDateTime.Ticks / TimeSpan.TicksPerSecond;
            return seconds - unixEpochSeconds;
        }

        public static DateTimeOffset RoundToClosestMinute(this DateTimeOffset time)
            => time.Second >= (SecondsInAMinute / 2)
                ? time + TimeSpan.FromSeconds(SecondsInAMinute - time.Second)
                : time - TimeSpan.FromSeconds(time.Second);

        public static DateTimeOffset RoundDownToMinute(this DateTimeOffset time)
            => new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0, time.Offset);

        public static DateTimeOffset WithDate(this DateTimeOffset original, DateTimeOffset date)
            => new DateTimeOffset(date.Year, date.Month, date.Day,
                                  original.Hour, original.Minute, original.Second, original.Offset);

        public static DateTimeOffset WithTime(this DateTimeOffset original, DateTimeOffset time)
            => new DateTimeOffset(original.Year, original.Month, original.Day,
                                  time.Hour, time.Minute, time.Second, original.Offset);

        public static DateTimeOffset RoundDownToLocalDate(this DateTimeOffset time)
        {
            var localTime = time.ToOffset(DateTimeOffset.Now.Offset);
            return new DateTimeOffset(localTime.Date, localTime.Offset);
        }

        public static DateTimeOffset RoundDownToLocalMonth(this DateTimeOffset time)
        {
            var localTime = time.ToOffset(DateTimeOffset.Now.Offset);
            return new DateTimeOffset(localTime.Year, localTime.Month, 1, 0, 0, 0, localTime.Offset);
        }

        public static DateTimeOffset RoundDownToLocalYear(this DateTimeOffset time)
        {
            var localTime = time.ToOffset(DateTimeOffset.Now.Offset);
            return new DateTimeOffset(localTime.Year, 1, 1, 0, 0, 0, localTime.Offset);
        }

        public static bool IsCurrentWeek(this (DateTimeOffset Start, DateTimeOffset End) range, DateTimeOffset currentTime, BeginningOfWeek beginningOfWeek)
        {
            var firstDayOfCurrentWeek = currentTime.BeginningOfWeek(beginningOfWeek);
            var lastDayOfCurrentWeek = firstDayOfCurrentWeek.AddDays(6);
            return range.Start == firstDayOfCurrentWeek && range.End == lastDayOfCurrentWeek;
        }

        public static bool IsLastWeek(this (DateTimeOffset Start, DateTimeOffset End) range, DateTimeOffset currentTime, BeginningOfWeek beginningOfWeek)
        {
            var firstDayOfLastWeek = currentTime.BeginningOfWeek(beginningOfWeek).AddDays(-7);
            var lastDayOfLastWeek = firstDayOfLastWeek.AddDays(6);
            return range.Start == firstDayOfLastWeek && range.End == lastDayOfLastWeek;
        }

        public static bool IsCurrentMonth(this (DateTimeOffset Start, DateTimeOffset End) range, DateTimeOffset currentTime)
        {
            var firstDayOfCurrentMonth = currentTime.RoundDownToLocalMonth();
            var lastDayOfCurrentMonth = firstDayOfCurrentMonth.AddMonths(1).AddDays(-1);
            return range.Start == firstDayOfCurrentMonth && range.End == lastDayOfCurrentMonth;
        }

        public static bool IsLastMonth(this (DateTimeOffset Start, DateTimeOffset End) range, DateTimeOffset currentTime)
        {
            var firstDayOfPreviousMonth = currentTime.RoundDownToLocalMonth().AddMonths(-1);
            var lastDayOfPreviousMonth = firstDayOfPreviousMonth.AddMonths(1).AddDays(-1);
            return range.Start == firstDayOfPreviousMonth && range.End == lastDayOfPreviousMonth;
        }

        public static bool IsCurrentYear(this (DateTimeOffset Start, DateTimeOffset End) range, DateTimeOffset currentTime)
        {
            var firstDayOfCurrentYear = currentTime.RoundDownToLocalYear();
            var lastDayOfCurrentYear = firstDayOfCurrentYear.AddYears(1).AddDays(-1);
            return range.Start == firstDayOfCurrentYear && range.End == lastDayOfCurrentYear;
        }

        public static bool IsLastYear(this (DateTimeOffset Start, DateTimeOffset End) range, DateTimeOffset currentTime)
        {
            var firstDayOfLastYear = currentTime.RoundDownToLocalYear().AddYears(-1);
            var lastDayOfLastYear = firstDayOfLastYear.AddYears(1).AddDays(-1);
            return range.Start == firstDayOfLastYear && range.End == lastDayOfLastYear;
        }

        public static DateTimeOffset RoundToClosestQuarter(this DateTimeOffset time)
        {
            var roundDown = RoundDownToClosestQuarter(time);
            var roundUp = RoundUpToClosestQuarter(time);

            if (SystemMath.Abs((roundDown.RoundDownToClosestQuarter() - time).TotalSeconds) > SystemMath.Abs((roundUp.RoundDownToClosestQuarter() - time).TotalSeconds))
            {
                return roundUp;
            }
            else
            {
                return roundDown;
            }
        }

        public static DateTimeOffset RoundDownToClosestQuarter(this DateTimeOffset time)
        {
            var offset = time.Minute % 15;
            var minute = time.Minute - offset;
            return new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, minute, 0, time.Offset);
        }

        public static DateTimeOffset RoundUpToClosestQuarter(this DateTimeOffset time)
        {
            if (time.Minute >= 45)
            {
                var nextHour = time.AddHours(1);
                return new DateTimeOffset(nextHour.Year, nextHour.Month, nextHour.Day, nextHour.Hour, 0, 0, nextHour.Offset);
            }
            else
            {
                var offset = 15 - time.Minute % 15;
                var minute = time.Minute + offset;
                return new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, minute, 0, time.Offset);
            }
        }

        public static DateTimeOffset BeginningOfWeek(this DateTimeOffset time, BeginningOfWeek beginningOfWeek)
        {
            var localTime = time.ToOffset(DateTimeOffset.Now.Offset);
            var localDayOfWeek = (BeginningOfWeek)localTime.DayOfWeek;
            int diff = (7 + (localDayOfWeek - beginningOfWeek)) % 7;
            var date = localTime.AddDays(-diff).Date;
            return new DateTimeOffset(date, localTime.Offset);
        }

        public static bool IsWithinTogglLimits(this DateTimeOffset time)
            => time.Year >= minAllowedYear && time.Year <= maxAllowedYear;
    }
}
