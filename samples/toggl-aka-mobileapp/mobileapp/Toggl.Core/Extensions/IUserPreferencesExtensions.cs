using System;
using System.Reactive.Linq;
using Toggl.Shared;
using Toggl.Storage.Settings;

namespace Toggl.Core.Extensions
{
    public static class IUserPreferencesExtensions
    {
        public static IObservable<CalendarNotificationsOption> CalendarNotificationsSettings(this IUserPreferences userPreferences)
        {
            var calendarNotificationsEnabled = userPreferences.CalendarNotificationsEnabled;
            var timeSpanForNotifications = userPreferences.TimeSpanBeforeCalendarNotifications;

            return Observable.CombineLatest(
                calendarNotificationsEnabled,
                timeSpanForNotifications,
                (enabled, timeSpan) =>
                {
                    if (!enabled)
                        return CalendarNotificationsOption.Disabled;

                    switch (timeSpan.TotalMinutes)
                    {
                        case 0:
                            return CalendarNotificationsOption.WhenEventStarts;
                        case 5:
                            return CalendarNotificationsOption.FiveMinutes;
                        case 10:
                            return CalendarNotificationsOption.TenMinutes;
                        case 15:
                            return CalendarNotificationsOption.FifteenMinutes;
                        case 30:
                            return CalendarNotificationsOption.ThirtyMinutes;
                        case 60:
                            return CalendarNotificationsOption.OneHour;
                    }
                    return CalendarNotificationsOption.Disabled;
                }
            );
        }
    }
}
