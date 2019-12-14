using System;

namespace Toggl.Shared
{
    public enum DurationFormat
    {
        Classic = 0,
        Improved = 1,
        Decimal = 2
    }

    public static class DurationFormatExtensions
    {
        public static string ToFormattedString(this DurationFormat durationFormat)
        {
            switch (durationFormat)
            {
                case DurationFormat.Classic:
                    return Resources.DurationFormatClassic;

                case DurationFormat.Improved:
                    return Resources.DurationFormatImproved;

                case DurationFormat.Decimal:
                    return Resources.DurationFormatDecimal;

                default:
                    throw new ArgumentException(
                        $"Duration format must be either: {nameof(DurationFormat.Classic)}, {nameof(DurationFormat.Improved)} or {nameof(DurationFormat.Decimal)}"
                    );
            }
        }
    }
}
