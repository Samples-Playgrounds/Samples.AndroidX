using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Accord.Statistics.Filters;
using Toggl.Core.Calendar;
using Toggl.Core.Exceptions;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;
using Math = System.Math;

namespace Toggl.Core.Suggestions
{
    public sealed class CalendarSuggestionProvider : ISuggestionProvider
    {
        private const int maxCalendarSuggestionCount = 1;

        private readonly ITimeService timeService;
        private readonly ICalendarService calendarService;
        private readonly IInteractorFactory interactorFactory;

        private readonly TimeSpan lookBackTimeSpan = TimeSpan.FromHours(1);
        private readonly TimeSpan lookAheadTimeSpan = TimeSpan.FromHours(1);

        public CalendarSuggestionProvider(
            ITimeService timeService,
            ICalendarService calendarService,
            IInteractorFactory interactorFactory)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(calendarService, nameof(calendarService));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.timeService = timeService;
            this.calendarService = calendarService;
            this.interactorFactory = interactorFactory;
        }

        public IObservable<Suggestion> GetSuggestions()
        {
            var now = timeService.CurrentDateTime;
            var startOfRange = now - lookBackTimeSpan;
            var endOfRange = now + lookAheadTimeSpan;

            var eventsObservable = calendarService
                .GetEventsInRange(startOfRange, endOfRange)
                .Select(events => events.Where(eventHasDescription))
                .SelectMany(orderByOffset);

            var selectedUserCalendars = interactorFactory
                .GetUserCalendars()
                .Execute()
                .Select(calendars => calendars.Where(c => c.IsSelected));

            var eventsFromSelectedUserCalendars = eventsObservable.WithLatestFrom(selectedUserCalendars,
                    (calendarItem, calendars) =>
                        (calendarItem: calendarItem, userCalendarIds: calendars.Select(c => c.Id)))
                .Where(tuple => tuple.userCalendarIds.Contains(tuple.calendarItem.CalendarId))
                .Select(tuple => tuple.calendarItem);

            return interactorFactory.GetDefaultWorkspace().Execute()
                .CombineLatest(
                    eventsFromSelectedUserCalendars,
                    (workspace, calendarItem) => suggestionFromEvent(calendarItem, workspace.Id))
                .Take(maxCalendarSuggestionCount)
                .OnErrorResumeEmpty();
        }

        private Suggestion suggestionFromEvent(CalendarItem calendarItem, long workspaceId)
            => new Suggestion(calendarItem, workspaceId, SuggestionProviderType.Calendar);

        private bool eventHasDescription(CalendarItem calendarItem)
            => !string.IsNullOrWhiteSpace(calendarItem.Description);

        private TimeSpan absOffset(CalendarItem item)
        {
            var currentTime = timeService.CurrentDateTime;
            var startTime = item.StartTime;
            return (currentTime - startTime).Duration();
        }

        private IEnumerable<CalendarItem> orderByOffset(IEnumerable<CalendarItem> calendarItems)
            => calendarItems.OrderBy(absOffset);
    }
}
