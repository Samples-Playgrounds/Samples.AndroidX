using EventKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Calendar;
using Toggl.Core.UI.Services;
using Toggl.iOS.Extensions;
using Toggl.Shared;

namespace Toggl.iOS.Services
{
    public sealed class CalendarServiceIos : PermissionAwareCalendarService
    {
        private EKEventStore eventStore = new EKEventStore();

        public CalendarServiceIos(IPermissionsChecker permissionsChecker)
            : base(permissionsChecker)
        {
        }

        protected override IEnumerable<CalendarItem> NativeGetEventsInRange(DateTimeOffset start, DateTimeOffset end)
        {
            var calendars = eventStore.GetCalendars(EKEntityType.Event);

            var predicate = eventStore
                .PredicateForEvents(start.ToNSDate(), end.ToNSDate(), calendars);

            var calendarItems = eventStore
                .EventsMatching(predicate)
                ?.Where(isValidEvent)
                ?.Select(calendarItemFromEvent)
                ?? new CalendarItem[0];

            return calendarItems;
        }

        protected override IEnumerable<UserCalendar> NativeGetUserCalendars()
            => eventStore
                .GetCalendars(EKEntityType.Event)
                .Select(userCalendarFromEKCalendar);

        protected override CalendarItem NativeGetCalendarItemWithId(string id)
        {
            var calendarEvent = eventStore.EventFromIdentifier(id);
            if (calendarEvent == null)
                throw new InvalidOperationException("An invalid calendar Id was provided");

            var calendarItem = calendarItemFromEvent(calendarEvent);
            return calendarItem;
        }

        private UserCalendar userCalendarFromEKCalendar(EKCalendar calendar)
            => new UserCalendar(
                calendar.CalendarIdentifier,
                calendar.Title,
                calendar.Source.Title);

        private CalendarItem calendarItemFromEvent(EKEvent ev)
        {
            var startDate = ev.StartDate.ToDateTimeOffset();
            var endDate = ev.EndDate.ToDateTimeOffset();
            var duration = endDate - startDate;

            return new CalendarItem(
                ev.EventIdentifier,
                CalendarItemSource.Calendar,
                startDate,
                duration,
                ev.Title,
                CalendarIconKind.Event,
                ev.Calendar.CGColor.ToHexColor(),
                calendarId: ev.Calendar.CalendarIdentifier
            );
        }

        private bool isValidEvent(EKEvent ev)
            => !ev.AllDay;
    }
}
