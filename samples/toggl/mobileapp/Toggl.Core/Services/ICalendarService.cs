using System;
using System.Collections.Generic;
using Toggl.Core.Calendar;
using Toggl.Shared;

namespace Toggl.Core.Services
{
    public interface ICalendarService
    {
        IObservable<CalendarItem> GetEventWithId(string id);

        IObservable<IEnumerable<CalendarItem>> GetEventsForDate(DateTime date);

        IObservable<IEnumerable<CalendarItem>> GetEventsInRange(DateTimeOffset start, DateTimeOffset end);

        IObservable<IEnumerable<UserCalendar>> GetUserCalendars();
    }
}
