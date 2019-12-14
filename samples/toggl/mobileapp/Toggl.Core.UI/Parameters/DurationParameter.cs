using System;

namespace Toggl.Core.UI.Parameters
{
    public sealed class DurationParameter
    {
        public DateTimeOffset Start { get; set; }

        public TimeSpan? Duration { get; set; }

        public static DurationParameter WithStartAndDuration(DateTimeOffset start, TimeSpan? duration)
            => new DurationParameter { Start = start, Duration = duration };
    }
}
