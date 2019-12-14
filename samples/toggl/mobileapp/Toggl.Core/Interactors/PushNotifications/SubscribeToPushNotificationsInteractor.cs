using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Toggl.Core.Services;
using Toggl.Networking;
using Toggl.Networking.ApiClients;
using Toggl.Shared;
using Toggl.Storage;

namespace Toggl.Core.Interactors.PushNotifications
{
    public class SubscribeToPushNotificationsInteractor : IInteractor<IObservable<Unit>>
    {
        private readonly TimeSpan tokenReSubmissionPeriod = TimeSpan.FromDays(5);

        private readonly IPushNotificationsTokenStorage pushNotificationsTokenStorage;
        private readonly IPushServicesApi pushServicesApi;
        private readonly IPushNotificationsTokenService pushNotificationsTokenService;
        private readonly ITimeService timeService;

        public SubscribeToPushNotificationsInteractor(
            IPushNotificationsTokenStorage pushNotificationsTokenStorage,
            ITogglApi togglApi,
            IPushNotificationsTokenService pushNotificationsTokenService,
            ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(togglApi, nameof(togglApi));
            Ensure.Argument.IsNotNull(pushNotificationsTokenStorage, nameof(pushNotificationsTokenStorage));
            Ensure.Argument.IsNotNull(pushNotificationsTokenService, nameof(pushNotificationsTokenService));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.pushNotificationsTokenStorage = pushNotificationsTokenStorage;
            this.pushNotificationsTokenService = pushNotificationsTokenService;
            this.timeService = timeService;
            pushServicesApi = togglApi.PushServices;
        }

        public IObservable<Unit> Execute()
        {
            var token = pushNotificationsTokenService.Token;
            var defaultToken = default(PushNotificationsToken);
            if (!token.HasValue || token == defaultToken)
                return Observable.Return(Unit.Default);

            if (currentTokenWasAlreadySubmittedToTheServer(token.Value)
                && tokenWasSubmittedRecently())
            {
                return Observable.Return(Unit.Default);
            }

            return pushServicesApi
                .Subscribe(token.Value)
                .ToObservable()
                .Do(_ => pushNotificationsTokenStorage.StoreRegisteredToken(token.Value, timeService.CurrentDateTime))
                .Catch((Exception ex) => Observable.Return(Unit.Default));
        }

        private bool currentTokenWasAlreadySubmittedToTheServer(PushNotificationsToken currentToken)
        {
            var previouslyRegisteredToken = pushNotificationsTokenStorage.PreviouslyRegisteredToken?.ToString();
            return !string.IsNullOrEmpty(previouslyRegisteredToken) &&
                   previouslyRegisteredToken == currentToken.ToString();
        }

        private bool tokenWasSubmittedRecently()
        {
            var timeSinceLastSubmission =
                timeService.CurrentDateTime - pushNotificationsTokenStorage.DateOfRegisteringTheToken;
            return timeSinceLastSubmission < tokenReSubmissionPeriod;
        }
    }
}
