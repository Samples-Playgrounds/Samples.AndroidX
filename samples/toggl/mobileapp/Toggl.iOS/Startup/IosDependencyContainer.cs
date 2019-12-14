using Foundation;
using System;
using Toggl.Core;
using Toggl.Core.Analytics;
using Toggl.Core.Services;
using Toggl.Core.Shortcuts;
using Toggl.Core.UI;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.iOS.Presentation;
using Toggl.iOS.Services;
using Toggl.Networking;
using Toggl.Networking.Network;
using Toggl.Shared;
using Toggl.Storage;
using Toggl.Storage.Queries;
using Toggl.Storage.Realm;
using Toggl.Storage.Realm.Queries;
using Toggl.Storage.Settings;
using UIKit;

namespace Toggl.iOS
{
    public sealed class IosDependencyContainer : UIDependencyContainer
    {
        private const int numberOfSuggestions = 3;
        private const ApiEnvironment environment =
#if USE_PRODUCTION_API
            ApiEnvironment.Production;
#else
            ApiEnvironment.Staging;
#endif

        private readonly Lazy<SettingsStorage> settingsStorage;

        private readonly Lazy<RealmConfigurator> realmConfigurator
            = new Lazy<RealmConfigurator>(() => new RealmConfigurator());

        public CompositePresenter ViewPresenter { get; }
        public IntentDonationService IntentDonationService { get; }

        public new static IosDependencyContainer Instance { get; private set; }

        public static void EnsureInitialized(UIWindow window, AppDelegate appDelegate)
        {
            if (Instance != null)
                return;

            var version = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString();
            var viewPresenter = new CompositePresenter(
                new RootPresenter(window, appDelegate),
                new NavigationPresenter(window, appDelegate),
                new ModalDialogPresenter(window, appDelegate),
                new ModalCardPresenter(window, appDelegate),
                new PageSheetPresenter(window, appDelegate)
            );

            Instance = new IosDependencyContainer(viewPresenter, environment, Platform.Daneel, version);
            UIDependencyContainer.Instance = Instance;
        }

        private IosDependencyContainer(CompositePresenter viewPresenter, ApiEnvironment environment, Platform platform, string version)
            : base(environment, new UserAgent(platform.ToString(), version))
        {
            ViewPresenter = viewPresenter;
            IntentDonationService = new IntentDonationService(AnalyticsService);

            var appVersion = Version.Parse(version);

            settingsStorage = new Lazy<SettingsStorage>(() => new SettingsStorage(appVersion, KeyValueStorage));
        }

        protected override IAnalyticsService CreateAnalyticsService()
            => new AnalyticsServiceIos();

        protected override IBackgroundSyncService CreateBackgroundSyncService()
            => new BackgroundSyncServiceIos();

        protected override IFetchRemoteConfigService CreateFetchRemoteConfigService()
            => new FetchRemoteConfigServiceIos();

        protected override ICalendarService CreateCalendarService()
            => new CalendarServiceIos(PermissionsChecker);

        protected override ITogglDatabase CreateDatabase()
            => new Database(realmConfigurator.Value);

        protected override IKeyValueStorage CreateKeyValueStorage()
            => new UserDefaultsStorageIos();

        protected override ILicenseProvider CreateLicenseProvider()
            => new LicenseProviderIos();

        protected override INotificationService CreateNotificationService()
            => new NotificationServiceIos(PermissionsChecker, TimeService);

        protected override IPermissionsChecker CreatePermissionsChecker()
            => new PermissionsCheckerIos();

        protected override IPlatformInfo CreatePlatformInfo()
            => new PlatformInfoIos();

        protected override IQueryFactory CreateQueryFactory()
            => new RealmQueryFactory(() => Realms.Realm.GetInstance(realmConfigurator.Value.Configuration));

        protected override IPrivateSharedStorageService CreatePrivateSharedStorageService()
            => new PrivateSharedStorageServiceIos();

        protected override IRatingService CreateRatingService()
            => new RatingServiceIos();

        protected override ISchedulerProvider CreateSchedulerProvider()
            => new IOSSchedulerProvider();

        protected override IApplicationShortcutCreator CreateShortcutCreator()
            => new ApplicationShortcutCreator();

        protected override IPushNotificationsTokenService CreatePushNotificationsTokenService()
            => new PushNotificationsTokenServiceIos();

        protected override INavigationService CreateNavigationService()
            => new NavigationService(ViewPresenter, ViewModelLoader, AnalyticsService);

        protected override ILastTimeUsageStorage CreateLastTimeUsageStorage()
            => settingsStorage.Value;

        protected override IOnboardingStorage CreateOnboardingStorage()
            => settingsStorage.Value;

        protected override IUserPreferences CreateUserPreferences()
            => settingsStorage.Value;

        protected override IAccessRestrictionStorage CreateAccessRestrictionStorage()
            => settingsStorage.Value;

        protected override IAccessibilityService CreateAccessibilityService()
            => new AccessibilityServiceIos();

        protected override IWidgetsService CreateWidgetsService()
            => new WidgetsServiceIos(DataSource);
    }
}
