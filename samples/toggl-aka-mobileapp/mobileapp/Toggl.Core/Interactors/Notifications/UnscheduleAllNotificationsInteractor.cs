using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.Exceptions;
using Toggl.Core.Services;
using Toggl.Shared;

namespace Toggl.Core.Interactors.Notifications
{
    public class UnscheduleAllNotificationsInteractor : IInteractor<IObservable<Unit>>
    {
        private readonly INotificationService notificationService;

        public UnscheduleAllNotificationsInteractor(INotificationService notificationService)
        {
            Ensure.Argument.IsNotNull(notificationService, nameof(notificationService));

            this.notificationService = notificationService;
        }

        public IObservable<Unit> Execute()
            => notificationService
                .UnscheduleAllNotifications()
                .Catch<Unit, NotAuthorizedException>(
                    ex => Observable.Return(Unit.Default)
                );
    }
}
