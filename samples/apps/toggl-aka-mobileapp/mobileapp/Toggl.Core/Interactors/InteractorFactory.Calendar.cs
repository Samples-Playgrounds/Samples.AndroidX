using System;
using System.Collections.Generic;
using System.Reactive;
using Toggl.Core.Calendar;
using Toggl.Core.Interactors.Calendar;
using Toggl.Shared;

namespace Toggl.Core.Interactors
{
    public sealed partial class InteractorFactory : IInteractorFactory
    {
        public IInteractor<IObservable<CalendarItem>> GetCalendarItemWithId(string eventId)
            => new GetCalendarItemWithIdInteractor(calendarService, eventId);

        public IInteractor<IObservable<IEnumerable<CalendarItem>>> GetCalendarItemsForDate(DateTime date)
            => new GetCalendarItemsForDateInteractor(dataSource.TimeEntries, calendarService, userPreferences, date);

        public IInteractor<IObservable<IEnumerable<UserCalendar>>> GetUserCalendars()
            => new GetUserCalendarsInteractor(calendarService, userPreferences);

        public IInteractor<Unit> SetEnabledCalendars(params string[] ids)
            => new SetEnabledCalendarsInteractor(userPreferences, ids);
    }
}
