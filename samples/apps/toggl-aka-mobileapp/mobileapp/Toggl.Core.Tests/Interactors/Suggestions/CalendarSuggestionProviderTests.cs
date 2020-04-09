using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Calendar;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.Suggestions;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.Suggestions
{
    public sealed class CalendarSuggestionProviderTests
    {
        public abstract class CalendarSuggestionProviderTest
        {
            protected CalendarSuggestionProvider Provider { get; }

            protected ITimeService TimeService { get; } = Substitute.For<ITimeService>();
            protected ICalendarService CalendarService { get; } = Substitute.For<ICalendarService>();
            protected IInteractorFactory InteractorFactory { get; } = Substitute.For<IInteractorFactory>();

            public CalendarSuggestionProviderTest()
            {
                Provider = new CalendarSuggestionProvider(TimeService, CalendarService, InteractorFactory);
            }
        }

        public sealed class TheConstructor : CalendarSuggestionProviderTest
        {
            [Xunit.Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useTimeService,
                bool useCalendarService,
                bool useInteractorFactory)
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new CalendarSuggestionProvider(
                        useTimeService ? TimeService : null,
                        useCalendarService ? CalendarService : null,
                        useInteractorFactory ? InteractorFactory : null
                    );

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheGetSuggestionsMethod : CalendarSuggestionProviderTest
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsSuggestionsFromEventsOneHourInThePastAndOneHourInTheFuture()
            {
                var now = new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                var tenMinutes = TimeSpan.FromMinutes(10);
                var events = Enumerable.Range(1, 5)
                    .Select(id => new CalendarItem(
                        id.ToString(),
                        CalendarItemSource.Calendar,
                        now - tenMinutes * id,
                        tenMinutes,
                        id.ToString(),
                        CalendarIconKind.None));
                CalendarService
                    .GetEventsInRange(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
                    .Returns(Observable.Return(events));

                var userCalendar = new UserCalendar(
                    "",
                    "",
                    "",
                    true);

                InteractorFactory.GetUserCalendars().Execute()
                    .Returns(Observable.Return(new List<UserCalendar> { userCalendar }));

                var suggestions = await Provider.GetSuggestions().ToList();

                await CalendarService.Received().GetEventsInRange(now.AddHours(-1), now.AddHours(1));
                suggestions.Should().HaveCount(1)
                    .And.OnlyContain(suggestion => events.Any(@event => @event.Description == suggestion.Description));
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsSuggestionsForTheDefaultWorkspace()
            {
                var defaultWorkspace = new MockWorkspace(10);
                var now = new DateTimeOffset(2020, 10, 9, 8, 7, 6, TimeSpan.Zero);
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(defaultWorkspace));
                TimeService.CurrentDateTime.Returns(now);
                var tenMinutes = TimeSpan.FromMinutes(10);
                var events = Enumerable.Range(1, 5)
                    .Select(id => new CalendarItem(
                        id.ToString(),
                        CalendarItemSource.Calendar,
                        now - tenMinutes * id,
                        tenMinutes,
                        id.ToString(),
                        CalendarIconKind.None));
                CalendarService
                    .GetEventsInRange(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
                    .Returns(Observable.Return(events));

                var userCalendar = new UserCalendar(
                    "",
                    "",
                    "",
                    true);

                InteractorFactory.GetUserCalendars().Execute()
                    .Returns(Observable.Return(new List<UserCalendar> { userCalendar }));

                var suggestions = await Provider.GetSuggestions().ToList();

                suggestions.Should().OnlyContain(suggestion => suggestion.WorkspaceId == defaultWorkspace.Id);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsSuggestionsSortedByABSOffsetFromNow()
            {
                var now = new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                var tenMinutes = TimeSpan.FromMinutes(10);
                var events = Enumerable.Range(1, 5)
                    .Select(id => new CalendarItem(
                        id.ToString(),
                        CalendarItemSource.Calendar,
                        id % 2 == 0 ? now - tenMinutes * id : now + tenMinutes * id,
                        tenMinutes,
                        id.ToString(),
                        CalendarIconKind.None));

                var expectedSuggestionIds = new List<string>
                {
                    "1",
                    "2",
                    "3",
                    "4",
                    "5"
                };

                var userCalendar = new UserCalendar(
                    "",
                    "",
                    "",
                    true);

                CalendarService
                    .GetEventsInRange(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
                    .Returns(Observable.Return(events));

                InteractorFactory.GetUserCalendars().Execute()
                    .Returns(Observable.Return(new List<UserCalendar> { userCalendar }));

                var suggestions = await Provider.GetSuggestions().ToList();

                await CalendarService.Received().GetEventsInRange(now.AddHours(-1), now.AddHours(1));
                suggestions.Should().HaveCount(1)
                    .And.OnlyContain(suggestion => expectedSuggestionIds.Any(expectedId => expectedId == suggestion.Description));
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsSuggestionsFromUserSelectedCalendars()
            {
                var now = new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                var tenMinutes = TimeSpan.FromMinutes(10);
                var events = Enumerable.Range(1, 5)
                    .Select(id => new CalendarItem(
                        id.ToString(),
                        CalendarItemSource.Calendar,
                        now - tenMinutes * id,
                        tenMinutes,
                        id.ToString(),
                        CalendarIconKind.None,
                        calendarId: id.ToString()));

                var userCalendars = Enumerable.Range(2, 3)
                    .Select(id => new UserCalendar(
                        id.ToString(),
                        id.ToString(),
                        "",
                        true));

                CalendarService
                    .GetEventsInRange(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
                    .Returns(Observable.Return(events));
                InteractorFactory.GetUserCalendars().Execute().Returns(Observable.Return(userCalendars));

                var suggestions = await Provider.GetSuggestions().ToList();

                await CalendarService.Received().GetEventsInRange(now.AddHours(-1), now.AddHours(1));
                suggestions.Should().HaveCount(1)
                    .And.OnlyContain(suggestion => events.Any(@event => userCalendars.Select(c => c.Id).Contains(@event.CalendarId)));
            }

            [Fact]
            public void NeverThrows()
            {
                var now = new DateTimeOffset(2020, 1, 5, 3, 55, 0, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                var exception = new Exception();
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Throw<IThreadSafeWorkspace>(exception));
                var provider = new CalendarSuggestionProvider(TimeService, CalendarService, InteractorFactory);

                Action getSuggestions = () => provider.GetSuggestions().Subscribe();
                getSuggestions.Should().NotThrow();
            }

            [Fact]
            public void ReturnsNoSuggestionsInCaseOfError()
            {
                var now = new DateTimeOffset(2020, 1, 5, 3, 55, 0, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                var exception = new Exception();
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Throw<IThreadSafeWorkspace>(exception));
                var provider = new CalendarSuggestionProvider(TimeService, CalendarService, InteractorFactory);

                provider.GetSuggestions().Count().Wait().Should().Be(0);
            }
        }
    }
}
