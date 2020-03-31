using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Android.App;
using Firebase.Iid;
using Toggl.Core.Extensions;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class TogglFirebaseIIDService : FirebaseInstanceIdService
    {
        private CompositeDisposable disposeBag = new CompositeDisposable();

        public override void OnTokenRefresh()
        {
            AndroidDependencyContainer.EnsureInitialized(Application.Context);

            var dependencyContainer = AndroidDependencyContainer.Instance;
            registerTokenIfNecessary(dependencyContainer);
        }

        private void registerTokenIfNecessary(AndroidDependencyContainer dependencyContainer)
        {
            var userLoggedIn = dependencyContainer.UserAccessManager.CheckIfLoggedIn();
            if (!userLoggedIn) return;

            var shouldBeSubscribedToPushNotifications = dependencyContainer
                .RemoteConfigService
                .GetPushNotificationsConfiguration()
                .RegisterPushNotificationsTokenWithServer;
            
            if (!shouldBeSubscribedToPushNotifications) return;

            dependencyContainer.InteractorFactory.SubscribeToPushNotifications()
                .Execute()
                .ObserveOn(dependencyContainer.SchedulerProvider.BackgroundScheduler)
                .Subscribe(_ => StopSelf())
                .DisposedBy(disposeBag);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            disposeBag?.Dispose();
        }
    }
}
