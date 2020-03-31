using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.Calendar;
using Toggl.Core.Exceptions;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;
using Notification = Toggl.Shared.Notification;

namespace Toggl.Core.Interactors.Notifications
{
    public sealed class ScheduleEventNotificationsInteractor : IInteractor<IObservable<Unit>>
    {
        private readonly TimeSpan period = TimeSpan.FromDays(7);

        private readonly ITimeService timeService;
        private readonly ICalendarService calendarService;
        private readonly IUserPreferences userPreferences;
        private readonly INotificationService notificationService;

        public ScheduleEventNotificationsInteractor(
            ITimeService timeService,
            ICalendarService calendarService,
            IUserPreferences userPreferences,
            INotificationService notificationService)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(calendarService, nameof(calendarService));
            Ensure.Argument.IsNotNull(notificationService, nameof(notificationService));

            this.timeService = timeService;
            this.calendarService = calendarService;
            this.userPreferences = userPreferences;
            this.notificationService = notificationService;
        }

        public IObservable<Unit> Execute()
        {
            var start = timeService.CurrentDateTime;
            var end = start.Add(period);
            var enabledCalendarIds = userPreferences.EnabledCalendarIds().ToHashSet();

            return userPreferences
                .TimeSpanBeforeCalendarNotifications
                .SelectMany(intervalBeforeEvent => calendarService
                    .GetEventsInRange(start, end)
                    .Select(calendarItems => calendarItems
                        .Where(calendarItem => enabledCalendarIds.Contains(calendarItem.CalendarId))
                        .Where(startTimeIsInTheFuture(start + intervalBeforeEvent))
                        .Select(eventNotificationWithOffset(intervalBeforeEvent))
                        .ToImmutableList())
                    .SelectMany(notificationService.Schedule)
                    .Catch<Unit, NotAuthorizedException>(ex => Observable.Return(Unit.Default))
                );
        }

        private Func<CalendarItem, bool> startTimeIsInTheFuture(DateTimeOffset start)
            => calendarItem => calendarItem.StartTime >= start;

        private Func<CalendarItem, Notification> eventNotificationWithOffset(TimeSpan intervalBeforeEvent)
            => calendarItem => new Notification(
                calendarItem.Id,
                Resources.EventReminder,
                calendarItem.Description,
                calendarItem.StartTime - intervalBeforeEvent
           );
    }
}
