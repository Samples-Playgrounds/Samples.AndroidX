using System.Collections.Generic;
using Toggl.Core.Analytics;

namespace Toggl.iOS
{
    public class SiriIntentErrorTrackingEvent : ITrackableEvent
    {
        public string EventName => "SiriIntentError";
        private readonly string Message;

        public SiriIntentErrorTrackingEvent(string message)
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
