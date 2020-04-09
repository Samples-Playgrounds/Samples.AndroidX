using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Toggl.Core.Services;
using Toggl.Networking;
using Toggl.Networking.ApiClients;
using Toggl.Shared;
using Toggl.Storage;
using Toggl.Storage.Settings;

namespace Toggl.Core.Interactors
{
    internal class UnsubscribeFromPushNotificationsInteractor : IInteractor<IObservable<Unit>>
    {
        private readonly IPushNotificationsTokenService pushNotificationsTokenService;
        private readonly IPushNotificationsTokenStorage pushNotificationsTokenStorage;
        private readonly IPushServicesApi pushServicesApi;

        public UnsubscribeFromPushNotificationsInteractor(
            IPushNotificationsTokenService pushNotificationsTokenService,
            IPushNotificationsTokenStorage pushNotificationsTokenStorage,
            ITogglApi togglApi
        )
        {
            Ensure.Argument.IsNotNull(pushNotificationsTokenService, nameof(pushNotificationsTokenService));
            Ensure.Argument.IsNotNull(pushNotificationsTokenStorage, nameof(pushNotificationsTokenStorage));
            Ensure.Argument.IsNotNull(togglApi, nameof(togglApi));

            this.pushNotificationsTokenService = pushNotificationsTokenService;
            this.pushNotificationsTokenStorage = pushNotificationsTokenStorage;
            this.pushServicesApi = togglApi.PushServices;
        }

        public IObservable<Unit> Execute()
        {
            var currentToken = pushNotificationsTokenService.Token;
            var registeredToken = pushNotificationsTokenStorage.PreviouslyRegisteredToken;

            if (currentToken.HasValue && currentToken.Value == registeredToken)
            {
                return pushServicesApi.Unsubscribe(currentToken.Value)
                    .ToObservable()
                    .Catch<Unit, Exception>(_ => Observable.Return(Unit.Default))
                    .Do(_ => clearTokenReferences());
            }

            return Observable.Return(Unit.Default);
        }

        private void clearTokenReferences()
        {
            pushNotificationsTokenStorage.Clear();
            pushNotificationsTokenService.InvalidateCurrentToken();
        }
    }
}
