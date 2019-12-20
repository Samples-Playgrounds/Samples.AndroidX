using Android.App;
using Android.Content;
using System;
using Realms;
using Toggl.Core;
using Toggl.Core.Analytics;
using Toggl.Core.Services;
using Toggl.Core.Shortcuts;
using Toggl.Core.UI;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.Droid.Presentation;
using Toggl.Droid.Services;
using Toggl.Networking;
using Toggl.Networking.Network;
using Toggl.Shared;
using Toggl.Storage;
using Toggl.Storage.Queries;
using Toggl.Storage.Realm;
using Toggl.Storage.Realm.Queries;
using Toggl.Storage.Settings;

namespace Toggl.Droid
{
    public sealed class AndroidDependencyContainer : UIDependencyContainer
    {
        private const int numberOfSuggestions = 5;
        private const ApiEnvironment environment =
#if USE_PRODUCTION_API
                        ApiEnvironment.Production;
#else
                        ApiEnvironment.Staging;
#endif

        private readonly CompositePresenter viewPresenter;
        private readonly Lazy<SettingsStorage> settingsStorage;
        private readonly Lazy<RealmConfigurator> realmConfigurator
            = new Lazy<RealmConfigurator>(() => new RealmConfigurator());

        public ViewModelCache ViewModelCache { get; } = new ViewModelCache();

        public new static AndroidDependencyContainer Instance { get; private set; }

        public static void EnsureInitialized(Context context)
        {
            if (Instance != null)
                return;

            var packageInfo = context.PackageManager.GetPackageInfo(context.PackageName, 0);

            Instance = new AndroidDependencyContainer(environment, Platform.Giskard, packageInfo.VersionName);
            UIDependencyContainer.Instance = Instance;
        }

        private AndroidDependencyContainer(ApiEnvironment environment, Platform platform, string version)
            : base(environment, new UserAgent(platform.ToString(), version))
        {
            var appVersion = Version.Parse(version);

            viewPresenter = new CompositePresenter(new ActivityPresenter(), new DialogFragmentPresenter());
            settingsStorage = new Lazy<SettingsStorage>(() => new SettingsStorage(appVersion, KeyValueStorage));
        }

        protected override IAnalyticsService CreateAnalyticsService()
            => new AnalyticsServiceAndroid();

        protected override IBackgroundSyncService CreateBackgroundSyncService()
            => new BackgroundSyncServiceAndroid();

        protected override IFetchRemoteConfigService CreateFetchRemoteConfigService()
            => new FetchRemoteConfigServiceAndroid();

        protected override ICalendarService CreateCalendarService()
            => new CalendarServiceAndroid(PermissionsChecker);

        protected override ITogglDatabase CreateDatabase()
            => new Database(realmConfigurator.Value);

        protected override IKeyValueStorage CreateKeyValueStorage()
        {
            var sharedPreferences = Application.Context.GetSharedPreferences(Platform.Giskard.ToString(), FileCreationMode.Private);
            return new SharedPreferencesStorageAndroid(sharedPreferences);
        }

        protected override ILicenseProvider CreateLicenseProvider()
            => new LicenseProviderAndroid();

        protected override INotificationService CreateNotificationService()
            => new NotificationServiceAndroid();

        protected override IPermissionsChecker CreatePermissionsChecker()
            => new PermissionsCheckerAndroid();

        protected override IPlatformInfo CreatePlatformInfo()
            => new PlatformInfoAndroid();

        protected override IQueryFactory CreateQueryFactory()
            => new RealmQueryFactory(() => Realm.GetInstance(realmConfigurator.Value.Configuration));

        protected override IPrivateSharedStorageService CreatePrivateSharedStorageService()
            => new PrivateSharedStorageServiceAndroid(KeyValueStorage);

        protected override IRatingService CreateRatingService()
            => new RatingServiceAndroid(Application.Context);

        protected override ISchedulerProvider CreateSchedulerProvider()
            => new AndroidSchedulerProvider(AnalyticsService);

        protected override IApplicationShortcutCreator CreateShortcutCreator()
            => new ApplicationShortcutCreator(Application.Context);

        protected override IPushNotificationsTokenService CreatePushNotificationsTokenService()
            => new PushNotificationsTokenServiceAndroid();

        protected override INavigationService CreateNavigationService()
            => new NavigationService(
                viewPresenter,
                ViewModelLoader,
                AnalyticsService
            );

        protected override ILastTimeUsageStorage CreateLastTimeUsageStorage()
            => settingsStorage.Value;

        protected override IOnboardingStorage CreateOnboardingStorage()
            => settingsStorage.Value;

        protected override IUserPreferences CreateUserPreferences()
            => settingsStorage.Value;

        protected override IAccessRestrictionStorage CreateAccessRestrictionStorage()
            => settingsStorage.Value;

        protected override IAccessibilityService CreateAccessibilityService()
            => new AccessibilityServiceAndroid();

        protected override IWidgetsService CreateWidgetsService()
            => new WidgetsServiceAndroid(DataSource);
    }
}
