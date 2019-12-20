using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Services;
using Toggl.Shared;

namespace Toggl.Core.Calendar
{
    public abstract class PermissionAwareCalendarService : ICalendarService
    {
        private readonly IPermissionsChecker permissionsChecker;

        protected PermissionAwareCalendarService(IPermissionsChecker permissionsChecker)
        {
            Ensure.Argument.IsNotNull(permissionsChecker, nameof(permissionsChecker));

            this.permissionsChecker = permissionsChecker;
        }

        public IObservable<IEnumerable<CalendarItem>> GetEventsForDate(DateTime date)
        {
            var startOfDay = new DateTimeOffset(date.Date);
            var endOfDay = startOfDay.AddDays(1);

            return GetEventsInRange(startOfDay, endOfDay);
        }

        public IObservable<CalendarItem> GetEventWithId(string id)
            => permissionsChecker
                .CalendarPermissionGranted
                .DeferAndThrowIfPermissionNotGranted(
                    () => Observable.Return(NativeGetCalendarItemWithId(id))
                );

        public IObservable<IEnumerable<CalendarItem>> GetEventsInRange(DateTimeOffset start, DateTimeOffset end)
            => permissionsChecker
                .CalendarPermissionGranted
                .DeferAndThrowIfPermissionNotGranted(
                    () => Observable.Return(NativeGetEventsInRange(start, end))
                );

        public IObservable<IEnumerable<UserCalendar>> GetUserCalendars()
            => permissionsChecker
                .CalendarPermissionGranted
                .DeferAndThrowIfPermissionNotGranted(
                    () => Observable.Return(NativeGetUserCalendars())
                );

        protected abstract CalendarItem NativeGetCalendarItemWithId(string id);

        protected abstract IEnumerable<UserCalendar> NativeGetUserCalendars();

        protected abstract IEnumerable<CalendarItem> NativeGetEventsInRange(DateTimeOffset start, DateTimeOffset end);
    }
}
