using System;
using System.Reactive;
using Toggl.Core.Interactors.Notifications;

namespace Toggl.Core.Interactors
{
    public sealed partial class InteractorFactory : IInteractorFactory
    {
        public IInteractor<IObservable<Unit>> UnscheduleAllNotifications()
            => new UnscheduleAllNotificationsInteractor(notificationService);

        public IInteractor<IObservable<Unit>> ScheduleEventNotificationsForNextWeek()
            => new ScheduleEventNotificationsInteractor(timeService, calendarService, userPreferences, notificationService);
    }
}
