using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Calendar;
using Toggl.Core.Interactors.Calendar;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Toggl.Shared.Extensions;
using Toggl.Storage.Models;
using Xunit;

namespace Toggl.Core.Tests.Interactors.Calendar
{
    public sealed class GetCalendarItemsForDateInteractorTests
    {
        public sealed class TheExecuteMethod : BaseInteractorTests
        {
            private readonly DateTime date;
            private readonly GetCalendarItemsForDateInteractor interactor;

            private readonly List<CalendarItem> calendarEvents;

            private readonly List<IThreadSafeTimeEntry> timeEntries;
            private readonly List<CalendarItem> calendarItemsFromTimeEntries;

            public TheExecuteMethod()
            {
                calendarEvents = new List<CalendarItem>
                {
                    new CalendarItem(
                        "id",
                        CalendarItemSource.Calendar,
                        new DateTimeOffset(2018, 08, 06, 10, 30, 00, TimeSpan.Zero),
                        TimeSpan.FromMinutes(30),
                        "Important meeting",
                        CalendarIconKind.Event,
                        color: "#0000ff",
                        calendarId: "1"),
                    new CalendarItem(
                        "id",
                        CalendarItemSource.Calendar,
                        new DateTimeOffset(2018, 08, 06, 10, 00, 00, TimeSpan.Zero),
                        TimeSpan.FromMinutes(90),
                        "F**** timesheets",
                        CalendarIconKind.Event,
                        color: "#0000ff",
                        calendarId: "1"),
                    new CalendarItem(
                        "id",
                        CalendarItemSource.Calendar,
                        new DateTimeOffset(2018, 08, 06, 09, 00, 00, TimeSpan.Zero),
                        TimeSpan.FromMinutes(15),
                        "Not so important meeting",
                        CalendarIconKind.Event,
                        color: "#0000ff",
                        calendarId: "2")
                };

                timeEntries = new List<IThreadSafeTimeEntry>
                {
                    new MockTimeEntry()
                    {
                        Id = 1,
                        Description = "Something in project A",
                        Start = new DateTimeOffset(2018, 08, 06, 13, 00, 00, TimeSpan.Zero),
                        Duration = 45
                    },
                    new MockTimeEntry()
                    {
                        Id = 2,
                        Description = "Something in project B",
                        Start = new DateTimeOffset(2018, 08, 06, 11, 45, 00, TimeSpan.Zero),
                        Duration = 15
                    },
                    new MockTimeEntry()
                    {
                        Id = 3,
                        Description = "Something without project",
                        Start = new DateTimeOffset(2018, 08, 06, 09, 45, 00, TimeSpan.Zero),
                        Duration = 10
                    },
                    new MockTimeEntry()
                    {
                        Id = 4,
                        Description = "My deleted time entry",
                        Start = new DateTimeOffset(2018, 08, 06, 15, 45, 00, TimeSpan.Zero),
                        Duration = 10,
                        IsDeleted = true
                    },
                    new MockTimeEntry()
                    {
                        Id = 5,
                        Description = "Running time entry",
                        Start = new DateTimeOffset(2018, 08, 06, 15, 45, 00, TimeSpan.Zero),
                        Duration = null
                    }
                };

                calendarItemsFromTimeEntries = timeEntries
                    .Where(te => te.IsDeleted == false)
                    .Select(CalendarItem.From)
                    .ToList();

                date = new DateTime(2018, 08, 06);

                CalendarService
                    .GetEventsForDate(Arg.Is(date))
                    .Returns(Observable.Return(calendarEvents));

                DataSource
                    .TimeEntries
                    .GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>())
                    .Returns(callInfo =>
                    {
                        var filterFunc = callInfo.Arg<Func<IDatabaseTimeEntry, bool>>();
                        var filteredTimeEntries = timeEntries.Where(filterFunc).Cast<IThreadSafeTimeEntry>().ToList();
                        return Observable.Return(filteredTimeEntries);
                    });

                UserPreferences.EnabledCalendarIds().Returns(new[] { "1", "2" }.ToList());

                interactor = new GetCalendarItemsForDateInteractor(DataSource.TimeEntries, CalendarService, UserPreferences, date);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsAllNonDeletedCalendarItemsForTheGivenDate()
            {
                var calendarItems = await interactor.Execute();

                calendarItems.Should().Contain(calendarEvents);
                calendarItems.Should().Contain(calendarItemsFromTimeEntries);
                calendarItems.Count().Should().Be(calendarEvents.Count + calendarItemsFromTimeEntries.Count);
            }

            [Fact, LogIfTooSlow]
            public async Task RetunsAllCalendarItemsOrderedByStartDate()
            {
                var calendarItems = await interactor.Execute();

                calendarItems.Should().BeInAscendingOrder(calendarItem => calendarItem.StartTime);
            }

            [Fact, LogIfTooSlow]
            public async Task OnlyRetunsCalendarItemsThatComeFromEnabledCalendars()
            {
                UserPreferences.EnabledCalendarIds().Returns("1".Yield().ToList());

                var calendarItems = await interactor.Execute();

                calendarItems.Count().Should().Be(calendarEvents.Count + calendarItemsFromTimeEntries.Count - 1);
            }

            [Fact, LogIfTooSlow]
            public async Task FiltersElementsLongerThanTwentyFourHours()
            {
                var newCalendarEvents = calendarEvents
                    .Append(
                        new CalendarItem(
                            "id",
                            CalendarItemSource.Calendar,
                            new DateTimeOffset(2018, 08, 06, 10, 30, 00, TimeSpan.Zero),
                            TimeSpan.FromHours(24),
                            "Day off",
                            CalendarIconKind.Event,
                            "#0000ff"))
                    .Append(
                        new CalendarItem(
                            "id",
                            CalendarItemSource.Calendar,
                            new DateTimeOffset(2018, 08, 06, 10, 30, 00, TimeSpan.Zero),
                            TimeSpan.FromDays(7),
                            "Team meetup",
                            CalendarIconKind.Event,
                            "#0000ff")
                    );

                CalendarService
                    .GetEventsForDate(Arg.Is(date))
                    .Returns(Observable.Return(newCalendarEvents));

                var calendarItems = await interactor.Execute();

                calendarItems.Should().HaveCount(calendarItemsFromTimeEntries.Count + newCalendarEvents.Count() - 2);
                calendarItems.Should().NotContain(calendarItem => calendarItem.Description == "Day off" || calendarItem.Description == "Team meetup");
            }

            [Fact, LogIfTooSlow]
            public async Task IncludesTheRunningTimeEntry()
            {
                var calendarItems = await interactor.Execute();

                calendarItems.Should().Contain(calendarItem => calendarItem.Description == "Running time entry");
            }
        }
    }
}
