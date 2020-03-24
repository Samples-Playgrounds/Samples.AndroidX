using System;
using Toggl.Shared;
using static Toggl.Core.Helper.Constants;

namespace Toggl.Core.UI.Parameters
{
    [Preserve(AllMembers = true)]
    public sealed class DateTimePickerParameters
    {
        public DateTimePickerMode Mode { get; set; }

        public DateTimeOffset CurrentDate { get; set; }

        public DateTimeOffset MinDate { get; set; }

        public DateTimeOffset MaxDate { get; set; }

        public static DateTimePickerParameters WithDates(DateTimePickerMode mode, DateTimeOffset current, DateTimeOffset min, DateTimeOffset max)
        {
            if (min > max)
                throw new ArgumentException("Max date must be later than Min date.");

            return new DateTimePickerParameters
            {
                Mode = mode,
                CurrentDate = current,
                MinDate = min,
                MaxDate = max
            };
        }

        public static DateTimePickerParameters ForStartDateOfRunningTimeEntry(DateTimeOffset start, DateTimeOffset now)
            => WithDates(DateTimePickerMode.Date, start, now - MaxTimeEntryDuration, now);

        public static DateTimePickerParameters ForStartDateOfStoppedTimeEntry(DateTimeOffset start)
            => WithDates(DateTimePickerMode.Date, start, EarliestAllowedStartTime, LatestAllowedStartTime);
    }
}
