using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Exceptions;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;

namespace Toggl.Core.Interactors.Calendar
{
    public sealed class GetUserCalendarsInteractor : IInteractor<IObservable<IEnumerable<UserCalendar>>>
    {
        private readonly IUserPreferences userPreferences;
        private readonly ICalendarService calendarService;

        public GetUserCalendarsInteractor(ICalendarService calendarService, IUserPreferences userPreferences)
        {
            Ensure.Argument.IsNotNull(calendarService, nameof(calendarService));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));

            this.calendarService = calendarService;
            this.userPreferences = userPreferences;
        }

        public IObservable<IEnumerable<UserCalendar>> Execute()
        {
            var enabledIds = userPreferences.EnabledCalendarIds().ToHashSet();

            return calendarService
                .GetUserCalendars()
                .Select(calendarsWithTheSelectedProperty(enabledIds))
                .Catch<IEnumerable<UserCalendar>, NotAuthorizedException>(
                    ex => Observable.Return(new List<UserCalendar>())
                );
        }

        private Func<IEnumerable<UserCalendar>, IEnumerable<UserCalendar>> calendarsWithTheSelectedProperty(HashSet<string> enabledIds)
            => userCalendars => userCalendars.Select(calendar => calendar.WithSelected(enabledIds.Contains(calendar.Id)));
    }
}
