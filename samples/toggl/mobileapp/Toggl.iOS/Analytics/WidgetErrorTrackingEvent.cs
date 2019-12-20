using System.Collections.Generic;
using Toggl.Core.Analytics;

namespace Toggl.iOS
{
    public class WidgetErrorTrackingEvent : ITrackableEvent
    {
        public string EventName => "WidgetError";
        private readonly string Message;

        public WidgetErrorTrackingEvent(string message)
        {
            Message = message;
        }

        public Dictionary<string, string> ToDictionary()
            => new Dictionary<string, string>
            {
                ["Message"] = Message
            };
    }
}
