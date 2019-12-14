using System;
using System.Collections.Generic;

namespace Toggl.Shared
{
    public struct TimeFormat
    {
        private const string twelveHoursFormat = "h:mm A";
        private const string twentyFourHoursFormat = "H:mm";

        private static readonly Dictionary<string, string> formatConversion = new Dictionary<string, string>
        {
            [twelveHoursFormat] = "hh:mm tt",
            [twentyFourHoursFormat] = "H:mm"
        };

        public static TimeFormat TwelveHoursFormat { get; } = FromLocalizedTimeFormat(twelveHoursFormat);

        public static TimeFormat TwentyFourHoursFormat { get; } = FromLocalizedTimeFormat(twentyFourHoursFormat);

        /// <summary>
        /// Intended for displaying in the UI
        /// </summary>
        public string Localized { get; }

        /// <summary>
        /// Intended for  using in dateTime.ToString(useHere)
        /// </summary>
        public string Format { get; }

        public bool IsTwentyFourHoursFormat => Localized == twentyFourHoursFormat;

        private TimeFormat(string localized, string format)
        {
            Localized = localized;
            Format = format;
        }

        public static TimeFormat FromLocalizedTimeFormat(string timePattern)
        {
            if (formatConversion.ContainsKey(timePattern) == false)
                throw new ArgumentException($"Time pattern '{timePattern}' is not supported.");

            return new TimeFormat(timePattern, formatConversion[timePattern]);
        }
    }
}
