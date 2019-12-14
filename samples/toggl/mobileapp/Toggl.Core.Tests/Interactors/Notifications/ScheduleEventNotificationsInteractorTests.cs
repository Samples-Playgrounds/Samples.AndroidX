using FsCheck;
using FsCheck.Xunit;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Calendar;
using Toggl.Core.Interactors.Notifications;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.Interactors.Notifications
{
    public sealed class ScheduleEventNotificationsInteractorTests
    {
        public sealed class TheExecuteMethod : BaseInteractorTests
        {
            private readonly ScheduleEventNotificationsInteractor interactor;

            public TheExecuteMethod()
            {
                interactor = new ScheduleEventNotificationsInteractor(
                    TimeService,
                    CalendarService,
                    UserPreferences,
                    NotificationService
                );
            }

            [Fact, LogIfTooSlow]
            public async Task SchedulesNotificationsForAllUpcomingEventsInTheNextWeekThatBeginAfterTheCurrentDate()
            {
                var now = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
                var endOfWeek = now.AddDays(7);
                var eightHours = TimeSpan.FromHours(8);
                var tenMinutes = TimeSpan.FromMinutes(10);
                var events = Enumerable
                    .Range(0, 14)
                    .Select(number => new CalendarItem(
                        id: number.ToString(),
                        source: CalendarItemSource.Calendar,
                        startTime: now.Add(eightHours * number),
                        duration: eightHours,
                        description: number.ToString(),
                        iconKind: CalendarIconKind.None,
                        calendarId: "1"
                    ));
                var expectedNotifications = events
                    .Where(calendarItem => calendarItem.StartTime >= now + tenMinutes)
                    .Select(@event => new Notification(
                        @event.Id,
                        "Event reminder",
                        @event.Description,
                        @event.StartTime - tenMinutes
                    ));
                UserPreferences
                    .TimeSpanBeforeCalendarNotifications
                    .Returns(Observable.Return(tenMinutes));
                CalendarService
                    .GetEventsInRange(now, endOfWeek)
                    .Returns(Observable.Return(events));
                TimeService.CurrentDateTime.Returns(now);
                UserPreferences.EnabledCalendarIds().Returns(new List<string> { "1" });

                await interactor.Execute();

                await NotificationService
                    .Received()
                    .Schedule(Arg.Is<IImmutableList<Notification>>(
                        notifications => notifications.SequenceEqual(expectedNotifications))
                    );
            }

            [Property]
            public void SubtractsTheUserConfiguredTimeSpanFromTheEventsStartDate(long ticks)
            {
                if (ticks < 0) return;

                var timeSpan = TimeSpan.FromTicks(ticks);

                var now = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
                var endOfWeek = now.AddDays(7);
                var eightHours = TimeSpan.FromHours(8);
                var events = Enumerable
                    .Range(0, 14)
                    .Select(number => new CalendarItem(
                        id: number.ToString(),
                        source: CalendarItemSource.Calendar,
                        startTime: now.Add(eightHours * number),
                        duration: eightHours,
                        description: number.ToString(),
                        iconKind: CalendarIconKind.None,
                        calendarId: "1"
                    ));
                var expectedNotifications = events
                    .Where(calendarItem => calendarItem.StartTime >= now + timeSpan)
                    .Select(@event => new Notification(
                        @event.Id,
                        "Event reminder",
                        @event.Description,
                        @event.StartTime - timeSpan
                    ));
                UserPreferences
                    .TimeSpanBeforeCalendarNotifications
                    .Returns(Observable.Return(timeSpan));
                UserPreferences
                    .EnabledCalendarIds()
                    .Returns(new List<string> { "1" });
                CalendarService
                    .GetEventsInRange(now, endOfWeek)
                    .Returns(Observable.Return(events));
                TimeService.CurrentDateTime.Returns(now);
                NotificationService.ClearReceivedCalls();

                interactor.Execute().Wait();

                NotificationService
                    .Received()
                    .Schedule(Arg.Is<IImmutableList<Notification>>(
                        notifications => notifications.SequenceEqual(expectedNotifications))
                    );
            }

            [Fact, LogIfTooSlow]
            public async Task SchedulesEventsOnlyFromEnabledCalendars()
            {
                const int eventsPerCalendar = 4;
                var now = new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero);
                IEnumerable<CalendarItem> eventsForCalendar(UserCalendar calendar)
                {
                    return Enumerable
                        .Range(1, eventsPerCalendar)
                        .Select(id => new CalendarItem(
                            id.ToString(),
                            CalendarItemSource.Calendar,
                            now.AddHours(id),
                            TimeSpan.FromHours(1),
                            "description",
                            CalendarIconKind.None,
                            calendarId: calendar.Id
                        ));
                }
                var calendars = Enumerable
                    .Range(0, 3)
                    .Select(id => id.ToString())
                    .Select(id => new UserCalendar(
                        id,
                        id,
                        "Does not matter"
                    ))
                    .ToArray();
                CalendarService.GetUserCalendars().Returns(Observable.Return(calendars));
                var events = calendars
                    .SelectMany(eventsForCalendar);
                CalendarService
                    .GetEventsInRange(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
                    .Returns(Observable.Return(events));
                UserPreferences.EnabledCalendarIds().Returns(new List<string> { "1" });
                TimeService.CurrentDateTime.Returns(now);

                await interactor.Execute();

                await NotificationService.Received().Schedule(Arg.Is<ImmutableList<Notification>>(
                    notifications => notifications.Count == eventsPerCalendar
                ));
            }
        }
    }
}
