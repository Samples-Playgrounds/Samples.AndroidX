using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl.Core.Analytics
{
    public class CalendarSuggestionContinuedEvent : ITrackableEvent
    {
        public string EventName => "CalendarSuggestionContinued";

        private readonly string offsetCategory;

        public CalendarSuggestionContinuedEvent(TimeSpan offset)
        {
            var direction = offset > TimeSpan.Zero ? "after" : "before";

            offset = offset.Duration();

            var text = "";

            if (offset < TimeSpan.FromMinutes(5))
                text = "<5";
            else if (offset < TimeSpan.FromMinutes(15))
                text = "5-15";
            else if (offset < TimeSpan.FromMinutes(30))
                text = "15-30";
            else if (offset < TimeSpan.FromMinutes(60))
                text = "30-60";
            else
                text = ">60";

            offsetCategory = $"{text} {direction}";
        }

        public Dictionary<string, string> ToDictionary() => new Dictionary<string, string>
        {
            ["Offset"] = offsetCategory
        };
    }
}
