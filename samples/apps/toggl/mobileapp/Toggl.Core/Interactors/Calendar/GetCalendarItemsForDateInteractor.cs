using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Calendar;
using Toggl.Core.DataSources;
using Toggl.Core.Exceptions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Storage.Settings;

namespace Toggl.Core.Interactors.Calendar
{
    public sealed class GetCalendarItemsForDateInteractor : IInteractor<IObservable<IEnumerable<CalendarItem>>>
    {
        private readonly TimeSpan maxDurationThreshold = TimeSpan.FromHours(24);

        private readonly ITimeEntriesSource timeEntriesDataSource;
        private readonly ICalendarService calendarService;
        private readonly IUserPreferences userPreferences;
        private readonly DateTime date;

        public GetCalendarItemsForDateInteractor(
            ITimeEntriesSource timeEntriesDataSource,
            ICalendarService calendarService,
            IUserPreferences userPreferences,
            DateTime date)
        {
            Ensure.Argument.IsNotNull(timeEntriesDataSource, nameof(timeEntriesDataSource));
            Ensure.Argument.IsNotNull(calendarService, nameof(calendarService));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(date, nameof(date));

            this.timeEntriesDataSource = timeEntriesDataSource;
            this.calendarService = calendarService;
            this.userPreferences = userPreferences;
            this.date = date;
        }

        public IObservable<IEnumerable<CalendarItem>> Execute()
            => Observable.CombineLatest(
                    calendarItemsFromTimeEntries(),
                    calendarItemsFromEvents(),
                    (timeEntries, events) => timeEntries.Concat(events))
                .Select(validEvents)
                .Select(orderByStartTime);

        private IObservable<IEnumerable<CalendarItem>> calendarItemsFromTimeEntries()
            => timeEntriesDataSource.GetAll(timeEntry
                    => timeEntry.IsDeleted == false
                    && timeEntry.Start >= date.Date
                    && timeEntry.Start <= date.AddDays(1).Date)
                .Select(timeEntries => timeEntries.ToList())
                .Select(convertTimeEntriesToCalendarItems);

        private IObservable<IEnumerable<CalendarItem>> calendarItemsFromEvents()
            => calendarService
                .GetEventsForDate(date)
                .Select(enabledCalendarItems)
                .Catch<IEnumerable<CalendarItem>, NotAuthorizedException>(
                    ex => Observable.Return(new List<CalendarItem>())
                );

        private IEnumerable<CalendarItem> enabledCalendarItems(IEnumerable<CalendarItem> calendarItems)
            => calendarItems.Where(userCalendarIsEnabled);

        private bool userCalendarIsEnabled(CalendarItem calendarItem)
            => userPreferences.EnabledCalendarIds().Contains(calendarItem.CalendarId);

        private IEnumerable<CalendarItem> convertTimeEntriesToCalendarItems(IEnumerable<IThreadSafeTimeEntry> timeEntries)
            => timeEntries.Select(CalendarItem.From);

        private IEnumerable<CalendarItem> validEvents(IEnumerable<CalendarItem> calendarItems)
            => calendarItems.Where(eventHasValidDuration);

        private bool eventHasValidDuration(CalendarItem calendarItem)
            => calendarItem.Duration == null || calendarItem.Duration < maxDurationThreshold;

        private IEnumerable<CalendarItem> orderByStartTime(IEnumerable<CalendarItem> calendarItems)
            => calendarItems.OrderBy(calendarItem => calendarItem.StartTime);
    }
}
