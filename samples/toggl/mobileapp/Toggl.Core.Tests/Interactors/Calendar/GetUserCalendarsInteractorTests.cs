using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Interactors.Calendar;
using Toggl.Core.Tests.Generators;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.Interactors.Calendar
{
    public sealed class GetUserCalendarsInteractorTests
    {
        public sealed class TheConstructor : BaseInteractorTests
        {
            [Theory]
            [ConstructorData]
            public void ThrowsIfTheArgumentIsNull(bool useCalendarService, bool useUserPreferences)
            {
                var calendarService = useCalendarService ? CalendarService : null;
                var userPreferences = useUserPreferences ? UserPreferences : null;

                Action tryingToConstructWithNulls =
                    () => new GetUserCalendarsInteractor(calendarService, userPreferences);

                tryingToConstructWithNulls.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheExecuteMethod : BaseInteractorTests
        {
            private static readonly IEnumerable<UserCalendar> calendarsFromService = new List<UserCalendar>
            {
                new UserCalendar("foo", "foo", "Google Calendar"),
                new UserCalendar("bar", "bar", "Google Calendar"),
                new UserCalendar("baz", "baz", "Google Calendar")
            };

            private static readonly IEnumerable<string> selectedCalendars = new List<string> { "foo", "bar" };

            public TheExecuteMethod()
            {
                var observable = Observable.Return(calendarsFromService);
                CalendarService.GetUserCalendars().Returns(observable);
                UserPreferences.EnabledCalendarIds().Returns(selectedCalendars);
            }

            [Fact]
            public async Task ReturnsAllCalendarsFromTheCalendarService()
            {
                var calendars = await InteractorFactory.GetUserCalendars().Execute();

                calendars.Should().HaveCount(calendarsFromService.Count());
            }

            [Fact]
            public async Task SetsTheCalendarsToSelectedWhenTheyWereSelectedByTheUser()
            {
                var calendars = await InteractorFactory.GetUserCalendars().Execute();

                calendars.Where(c => c.IsSelected).Should().HaveCount(selectedCalendars.Count());
            }
        }
    }
}
