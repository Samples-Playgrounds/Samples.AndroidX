using System;
using Toggl.Shared;

namespace Toggl.iOS.Extensions
{
    public static class NotificationExtensions
    {
        public static string GetIdentifier(this Notification notification)
        {
            var scheduledTime = notification.ScheduledTime;
            return $"{scheduledTime.DayOfWeek}{scheduledTime.Hour}{scheduledTime.Minute}{scheduledTime.Second}";
        }

        public static double GetTriggerTime(this Notification notification, DateTimeOffset now)
        {
            var diff = notification.ScheduledTime - now;
            return diff.TotalSeconds;
        }
    }
}
