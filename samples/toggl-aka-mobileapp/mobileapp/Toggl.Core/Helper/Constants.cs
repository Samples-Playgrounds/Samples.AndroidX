using System;

namespace Toggl.Core.Helper
{
    public static class Constants
    {
        public const int MaxTagNameLengthInBytes = 255;
        public const int MaxClientNameLengthInBytes = 255;
        public const int MaxProjectNameLengthInBytes = 255;
        public const int MaxTimeEntryDurationInHours = 999;
        public const int DefaultTimeEntryDurationForManualModeInMinutes = 30;
        public const int HoursPerDay = 24;

        public const int SinceDateLimitMonths = 2;
        public const int FetchTimeEntriesForMonths = 2;
        public const int TimeEntriesEndDateInclusiveExtraDaysCount = 2;

        public const string DefaultLanguageCode = "en";
        public static readonly string[] SupportedTwoLettersLanguageCodes = { "en", "ja" };

        public static readonly DateTimeOffset EarliestAllowedStartTime = new DateTimeOffset(2006, 1, 1, 0, 0, 0, TimeSpan.Zero);
        public static readonly DateTimeOffset LatestAllowedStartTime = new DateTimeOffset(2030, 12, 31, 23, 59, 59, TimeSpan.Zero);

        public static TimeSpan MaxTimeEntryDuration => TimeSpan.FromHours(MaxTimeEntryDurationInHours);

        public static TimeSpan UndoTime => TimeSpan.FromSeconds(5);

        public static TimeSpan CalendarItemViewDefaultDuration => TimeSpan.FromMinutes(30);
    }
}
