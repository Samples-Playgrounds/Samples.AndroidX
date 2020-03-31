using System;
using System.Reactive;
using Toggl.Core.Interactors.PushNotifications;

namespace Toggl.Core.Interactors
{
    public partial class InteractorFactory
    {
        public IInteractor<IObservable<Unit>> UnsubscribeFromPushNotifications()
            => new UnsubscribeFromPushNotificationsInteractor(pushNotificationsTokenService, lazyPushNotificationsTokenStorage.Value, api);

        public IInteractor<IObservable<Unit>> SubscribeToPushNotifications()
            => new SubscribeToPushNotificationsInteractor(lazyPushNotificationsTokenStorage.Value, api, pushNotificationsTokenService, timeService);
    }
}
