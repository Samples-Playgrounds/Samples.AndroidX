using System;
using System.Collections.Immutable;
using System.Reactive;
using Notification = Toggl.Shared.Notification;

namespace Toggl.Core.Services
{
    public interface INotificationService
    {
        IObservable<Unit> Schedule(IImmutableList<Notification> notifications);
        IObservable<Unit> UnscheduleAllNotifications();
    }
}
