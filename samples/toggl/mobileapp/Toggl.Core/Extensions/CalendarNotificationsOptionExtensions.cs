using System;
using Toggl.Shared;

namespace Toggl.Core.Extensions
{
    public static class CalendarNotificationsOptionExtensions
    {
        public static string Title(this CalendarNotificationsOption option)
        {
            switch (option)
            {
                case CalendarNotificationsOption.Disabled:
                    return Resources.Disabled;
                case CalendarNotificationsOption.WhenEventStarts:
                    return Resources.WhenEventStarts;
                case CalendarNotificationsOption.FiveMinutes:
                    return Resources.FiveMinutes;
                case CalendarNotificationsOption.TenMinutes:
                    return Resources.TenMinutes;
                case CalendarNotificationsOption.FifteenMinutes:
                    return Resources.FifteenMinutes;
                case CalendarNotificationsOption.ThirtyMinutes:
                    return Resources.ThirtyMinutes;
                case CalendarNotificationsOption.OneHour:
                    return Resources.OneHour;
            }
            return "";
        }

        public static TimeSpan Duration(this CalendarNotificationsOption option)
        {
            switch (option)
            {
                case CalendarNotificationsOption.WhenEventStarts:
                    return TimeSpan.Zero;
                case CalendarNotificationsOption.FiveMinutes:
                    return TimeSpan.FromMinutes(5);
                case CalendarNotificationsOption.TenMinutes:
                    return TimeSpan.FromMinutes(10);
                case CalendarNotificationsOption.FifteenMinutes:
                    return TimeSpan.FromMinutes(15);
                case CalendarNotificationsOption.ThirtyMinutes:
                    return TimeSpan.FromMinutes(30);
                case CalendarNotificationsOption.OneHour:
                    return TimeSpan.FromHours(1);
            }
            return TimeSpan.Zero;
        }
    }
}
