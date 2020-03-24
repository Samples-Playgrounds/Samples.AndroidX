using System.Collections.Generic;

namespace Toggl.Core.Analytics
{
    public sealed class StopTimerEntryEvent : ITrackableEvent
    {
        public string EventName => "TimeEntryStopped";
        public TimeEntryStopOrigin Origin { get; }

        public StopTimerEntryEvent(TimeEntryStopOrigin origin)
        {
            Origin = origin;
        }

        public Dictionary<string, string> ToDictionary() =>
            new Dictionary<string, string>
            {
                [nameof(Origin)] = Origin.ToString(),
            };
    }
}
