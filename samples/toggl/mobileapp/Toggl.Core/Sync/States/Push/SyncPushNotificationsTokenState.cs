using System;
using System.Reactive;
using Toggl.Core.Interactors;
using Toggl.Core.Interactors.PushNotifications;
using Toggl.Core.Services;
using Toggl.Networking;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;

namespace Toggl.Core.Sync.States.Push
{
    public sealed class SyncPushNotificationsTokenState : ISyncState
    {
        private readonly IPushNotificationsTokenStorage pushNotificationsTokenStorage;
        private readonly ITogglApi togglApi;
        private readonly IPushNotificationsTokenService pushNotificationsTokenService;
        private readonly ITimeService timeService;
        private readonly IRemoteConfigService remoteConfigService;

        public StateResult Done { get; } = new StateResult();

        public SyncPushNotificationsTokenState(
            IPushNotificationsTokenStorage pushNotificationsTokenStorage,
            ITogglApi togglApi,
            IPushNotificationsTokenService pushNotificationsTokenService,
            ITimeService timeService,
            IRemoteConfigService remoteConfigService)
        {
            Ensure.Argument.IsNotNull(pushNotificationsTokenStorage, nameof(pushNotificationsTokenStorage));
            Ensure.Argument.IsNotNull(togglApi, nameof(togglApi));
            Ensure.Argument.IsNotNull(pushNotificationsTokenService, nameof(pushNotificationsTokenService));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(remoteConfigService, nameof(remoteConfigService));

            this.pushNotificationsTokenStorage = pushNotificationsTokenStorage;
            this.togglApi = togglApi;
            this.pushNotificationsTokenService = pushNotificationsTokenService;
            this.timeService = timeService;
            this.remoteConfigService = remoteConfigService;
        }

        public IObservable<ITransition> Start()
            => handlePushNotificationTokenSubscription().SelectValue(Done.Transition());

        private IObservable<Unit> handlePushNotificationTokenSubscription()
            => remoteConfigService.GetPushNotificationsConfiguration().RegisterPushNotificationsTokenWithServer
                ? createSubscriptionInteractor().Execute()
                : createUnsubscriptionInteractor().Execute();

        private IInteractor<IObservable<Unit>> createSubscriptionInteractor()
            => new SubscribeToPushNotificationsInteractor(pushNotificationsTokenStorage, togglApi, pushNotificationsTokenService, timeService);

        private IInteractor<IObservable<Unit>> createUnsubscriptionInteractor()
            => new UnsubscribeFromPushNotificationsInteractor(pushNotificationsTokenService, pushNotificationsTokenStorage, togglApi);
    }
}
