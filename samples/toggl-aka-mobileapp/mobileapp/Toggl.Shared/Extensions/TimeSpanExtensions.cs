using System;
using System.Globalization;
using static System.FormattableString;

namespace Toggl.Shared.Extensions
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Positive(this TimeSpan duration)
            => duration < TimeSpan.Zero ? duration.Negate() : duration;
        
        public static string ToFormattedString(this TimeSpan duration, DurationFormat format)
        {
            switch (format)
            {
                case DurationFormat.Classic:
                    return convertToClassicFormat(duration);
                case DurationFormat.Improved:
                    return convertToImprovedFormat(duration);
                case DurationFormat.Decimal:
                    return convertToDecimalFormat(duration);
                default:
                    throw new ArgumentOutOfRangeException(
                        $"The duration converter parameter '{format}' is not of the supported formats.");
            }
        }

        private static string convertToDecimalFormat(TimeSpan value)
            => string.Format(CultureInfo.InvariantCulture, "{0:00.00} {1}", value.TotalHours, Resources.UnitHour);

        private static string convertToImprovedFormat(TimeSpan value)
            => Invariant($@"{(int)value.TotalHours}:{value:mm\:ss}");

        private static string convertToClassicFormat(TimeSpan value)
        {
            if (value >= TimeSpan.FromHours(1))
                return Invariant($@"{(int)value.TotalHours:00}:{value:mm\:ss}");

            if (value >= TimeSpan.FromMinutes(1))
                return Invariant($@"{value:mm\:ss} {Resources.UnitMin}");

            return Invariant($"{value:ss} {Resources.UnitSecond}");
        }
    }
}
