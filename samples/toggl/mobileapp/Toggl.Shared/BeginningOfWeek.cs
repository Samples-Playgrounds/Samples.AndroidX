using System;

namespace Toggl.Shared
{
    public enum BeginningOfWeek
    {
        Sunday = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6
    }

    public static class BeginningOfWeekExtensions
    {
        public static DayOfWeek ToDayOfWeekEnum(this BeginningOfWeek self)
            => (DayOfWeek)self;
    }
}
