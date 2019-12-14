using Android.App;
using Android.Database;
using System;
using System.Collections.Generic;
using Toggl.Core.Calendar;
using Toggl.Core.UI.Services;
using Toggl.Shared;
using static Android.Provider.CalendarContract;
using Color = Android.Graphics.Color;

namespace Toggl.Droid.Services
{
    public sealed class CalendarServiceAndroid : PermissionAwareCalendarService
    {
        private static readonly string[] calendarProjection =
        {
            Calendars.InterfaceConsts.Id,
            Calendars.InterfaceConsts.CalendarDisplayName,
            Calendars.InterfaceConsts.AccountName,
        };

        private static readonly string[] calendarIdProjection =
        {
            Calendars.InterfaceConsts.Id
        };

        private static readonly string[] eventsProjection =
        {
            Instances.InterfaceConsts.Id,
            Instances.Begin,
            Instances.End,
            Instances.InterfaceConsts.Title,
            Instances.InterfaceConsts.DisplayColor,
            Instances.InterfaceConsts.CalendarId,
            Instances.InterfaceConsts.AllDay
        };

        private const int calendarIdIndex = 0;
        private const int calendarDisplayNameIndex = 1;
        private const int calendarAccountNameIndex = 2;

        private const int eventIdIndex = 0;
        private const int eventStartDateIndex = 1;
        private const int eventEndDateIndex = 2;
        private const int eventDescriptionIndex = 3;
        private const int eventDisplayColorIndex = 4;
        private const int eventCalendarIdIndex = 5;
        private const int eventIsAllDayIndex = 6;

        public CalendarServiceAndroid(IPermissionsChecker permissionsChecker)
            : base(permissionsChecker)
        {
        }

        protected override IEnumerable<UserCalendar> NativeGetUserCalendars()
        {
            var appContext = Application.Context;

            var cursor = appContext.ContentResolver.Query(Calendars.ContentUri, calendarProjection, null, null, null);
            try
            {
                if (cursor.Count <= 0)
                    yield break;

                while (cursor.MoveToNext())
                {
                    var id = cursor.GetString(calendarIdIndex);
                    var displayName = cursor.GetString(calendarDisplayNameIndex);
                    var accountName = cursor.GetString(calendarAccountNameIndex);

                    yield return new UserCalendar(id, displayName, accountName);
                }
            }
            finally
            {
                cursor.Close();
            }
        }

        protected override IEnumerable<CalendarItem> NativeGetEventsInRange(DateTimeOffset start, DateTimeOffset end)
        {
            var appContext = Application.Context;

            var cursor = Instances.Query(appContext.ContentResolver, eventsProjection, start.ToUnixTimeMilliseconds(), end.ToUnixTimeMilliseconds());
            try
            {
                if (cursor.Count <= 0)
                    yield break;

                while (cursor.MoveToNext())
                {
                    var isAllDay = cursor.GetInt(eventIsAllDayIndex) == 1;
                    if (isAllDay)
                        continue;

                    yield return calendarItemFromCursor(cursor);
                }

            }
            finally
            {
                cursor.Close();
            }
        }

        protected override CalendarItem NativeGetCalendarItemWithId(string id)
        {
            var appContext = Application.Context;

            var cursor = appContext.ContentResolver.Query(Instances.ContentUri, eventsProjection, $"({Instances.InterfaceConsts.Id} = ?)", new[] { id }, null);
            if (cursor.Count <= 0)
            {
                cursor.Close();
                throw new InvalidOperationException("An invalid calendar Id was provided");
            }

            cursor.MoveToNext();
            var calendarItem = calendarItemFromCursor(cursor);
            cursor.Close();
            
            return calendarItem;
        }

        private static CalendarItem calendarItemFromCursor(ICursor cursor)
        {
            var id = cursor.GetString(eventIdIndex);
            var startDateUnixTime = cursor.GetLong(eventStartDateIndex);
            var endDateUnixTime = cursor.GetLong(eventEndDateIndex);
            var description = cursor.GetString(eventDescriptionIndex);
            var colorHex = cursor.GetInt(eventDisplayColorIndex);
            var calendarId = cursor.GetString(eventCalendarIdIndex);

            var startDate = DateTimeOffset.FromUnixTimeMilliseconds(startDateUnixTime);

            var color = new Color(colorHex);
            var rgb = $"#{color.R:X2}{color.G:X2}{color.B:X2}";

            return new CalendarItem(
                id: id,
                source: CalendarItemSource.Calendar,
                startTime: startDate,
                duration: DateTimeOffset.FromUnixTimeMilliseconds(endDateUnixTime) - startDate,
                description: description,
                iconKind: CalendarIconKind.Event,
                color: rgb,
                calendarId: calendarId
            );
        }
    }
}
