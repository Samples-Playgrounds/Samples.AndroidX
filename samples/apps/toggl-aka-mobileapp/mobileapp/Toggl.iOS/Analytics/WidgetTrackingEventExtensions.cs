using System;
using Toggl.Core.Analytics;
using Toggl.iOS.Shared.Analytics;

namespace Toggl.iOS
{
    public static class WidgetTrackingEventExtensions
    {
        public static ITrackableEvent ToTrackableEvent(this WidgetTrackingEvent e)
        {
            switch (e.EventType)
            {
                case WidgetTrackingEventType.Error:
                    return new SiriIntentErrorTrackingEvent(e.Parameters[WidgetErrorEventKeys.Message]);
                case WidgetTrackingEventType.StartTimer:
                    return new StartTimeEntryEvent(
                        TimeEntryStartOrigin.Widget,
                        true,
                        false,
                        false,
                        0,
                        false,
                        true
                        );
                case WidgetTrackingEventType.StopTimer:
                    return new StopTimerEntryEvent(TimeEntryStopOrigin.Widget);
                default:
                    return null;
            }
        }
    }
}
