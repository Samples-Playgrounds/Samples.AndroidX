using System;
using System.Collections.Immutable;
using System.Reactive;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Services;
using Notification = Toggl.Shared.Notification;

namespace Toggl.Core.Calendar
{
    public abstract class PermissionAwareNotificationService : INotificationService
    {
        private readonly IPermissionsChecker permissionsChecker;

        protected PermissionAwareNotificationService(IPermissionsChecker permissionsChecker)
        {
            this.permissionsChecker = permissionsChecker;
        }

        public IObservable<Unit> Schedule(IImmutableList<Notification> notifications)
            => permissionsChecker
                .NotificationPermissionGranted
                .DeferAndThrowIfPermissionNotGranted(
                    () => NativeSchedule(notifications)
                );

        public IObservable<Unit> UnscheduleAllNotifications()
            => permissionsChecker
                .NotificationPermissionGranted
                .DeferAndThrowIfPermissionNotGranted(NativeUnscheduleAllNotifications);

        protected abstract IObservable<Unit> NativeSchedule(IImmutableList<Notification> notifications);

        protected abstract IObservable<Unit> NativeUnscheduleAllNotifications();
    }
}
