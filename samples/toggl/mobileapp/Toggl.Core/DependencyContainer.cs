using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Login;
using Toggl.Core.Services;
using Toggl.Core.Shortcuts;
using Toggl.Core.Sync;
using Toggl.Networking;
using Toggl.Networking.Network;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Toggl.Storage.Queries;
using Toggl.Storage.Settings;

namespace Toggl.Core
{
    public abstract class DependencyContainer
    {
        private readonly UserAgent userAgent;
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        // Require recreation during login/logout
        private Lazy<ITogglApi> api;
        private Lazy<ITogglDataSource> dataSource;
        private Lazy<ISyncManager> syncManager;
        private Lazy<IInteractorFactory> interactorFactory;

        // Normal dependencies
        private readonly Lazy<IApiFactory> apiFactory;
        private readonly Lazy<ITogglDatabase> database;
        private readonly Lazy<ITimeService> timeService;
        private readonly Lazy<IPlatformInfo> platformInfo;
        private readonly Lazy<IQueryFactory> queryFactory;
        private readonly Lazy<IRatingService> ratingService;
        private readonly Lazy<ICalendarService> calendarService;
        private readonly Lazy<IKeyValueStorage> keyValueStorage;
        private readonly Lazy<ILicenseProvider> licenseProvider;
        private readonly Lazy<IUserPreferences> userPreferences;
        private readonly Lazy<IRxActionFactory> rxActionFactory;
        private readonly Lazy<IAnalyticsService> analyticsService;
        private readonly Lazy<IBackgroundService> backgroundService;
        private readonly Lazy<IOnboardingStorage> onboardingStorage;
        private readonly Lazy<ISchedulerProvider> schedulerProvider;
        private readonly Lazy<INotificationService> notificationService;
        private readonly Lazy<IRemoteConfigService> remoteConfigService;
        private readonly Lazy<IAccessibilityService> accessibilityService;
        private readonly Lazy<IErrorHandlingService> errorHandlingService;
        private readonly Lazy<ILastTimeUsageStorage> lastTimeUsageStorage;
        private readonly Lazy<IApplicationShortcutCreator> shortcutCreator;
        private readonly Lazy<IBackgroundSyncService> backgroundSyncService;
        private readonly Lazy<IAutomaticSyncingService> automaticSyncingService;
        private readonly Lazy<IAccessRestrictionStorage> accessRestrictionStorage;
        private readonly Lazy<ISyncErrorHandlingService> syncErrorHandlingService;
        private readonly Lazy<IFetchRemoteConfigService> fetchRemoteConfigService;
        private readonly Lazy<IUpdateRemoteConfigCacheService> remoteConfigUpdateService;
        private readonly Lazy<IPrivateSharedStorageService> privateSharedStorageService;
        private readonly Lazy<IPushNotificationsTokenService> pushNotificationsTokenService;
        private readonly Lazy<IPushNotificationsTokenStorage> pushNotificationsTokenStorage;

        // Non lazy
        public virtual IUserAccessManager UserAccessManager { get; }
        public ApiEnvironment ApiEnvironment { get; }

        public ISyncManager SyncManager => syncManager.Value;
        public IInteractorFactory InteractorFactory => interactorFactory.Value;

        public IApiFactory ApiFactory => apiFactory.Value;
        public ITogglDatabase Database => database.Value;
        public ITimeService TimeService => timeService.Value;
        public IQueryFactory QueryFactory => queryFactory.Value;
        public IPlatformInfo PlatformInfo => platformInfo.Value;
        public ITogglDataSource DataSource => dataSource.Value;
        public IRatingService RatingService => ratingService.Value;
        public IKeyValueStorage KeyValueStorage => keyValueStorage.Value;
        public ILicenseProvider LicenseProvider => licenseProvider.Value;
        public IUserPreferences UserPreferences => userPreferences.Value;
        public IRxActionFactory RxActionFactory => rxActionFactory.Value;
        public IAnalyticsService AnalyticsService => analyticsService.Value;
        public IBackgroundService BackgroundService => backgroundService.Value;
        public IOnboardingStorage OnboardingStorage => onboardingStorage.Value;
        public ISchedulerProvider SchedulerProvider => schedulerProvider.Value;
        public IRemoteConfigService RemoteConfigService => remoteConfigService.Value;
        public IAccessibilityService AccessibilityService => accessibilityService.Value;
        public IErrorHandlingService ErrorHandlingService => errorHandlingService.Value;
        public ILastTimeUsageStorage LastTimeUsageStorage => lastTimeUsageStorage.Value;
        public IBackgroundSyncService BackgroundSyncService => backgroundSyncService.Value;
        public IAutomaticSyncingService AutomaticSyncingService => automaticSyncingService.Value;
        public IAccessRestrictionStorage AccessRestrictionStorage => accessRestrictionStorage.Value;
        public ISyncErrorHandlingService SyncErrorHandlingService => syncErrorHandlingService.Value;
        public IFetchRemoteConfigService FetchRemoteConfigService => fetchRemoteConfigService.Value;
        public IUpdateRemoteConfigCacheService UpdateRemoteConfigCacheService => remoteConfigUpdateService.Value;
        public IPrivateSharedStorageService PrivateSharedStorageService => privateSharedStorageService.Value;
        public IPushNotificationsTokenService PushNotificationsTokenService => pushNotificationsTokenService.Value;
        public IPushNotificationsTokenStorage PushNotificationsTokenStorage => pushNotificationsTokenStorage.Value;

        protected DependencyContainer(ApiEnvironment apiEnvironment, UserAgent userAgent)
        {
            this.userAgent = userAgent;

            ApiEnvironment = apiEnvironment;

            database = new Lazy<ITogglDatabase>(CreateDatabase);
            apiFactory = new Lazy<IApiFactory>(CreateApiFactory);
            syncManager = new Lazy<ISyncManager>(CreateSyncManager);
            timeService = new Lazy<ITimeService>(CreateTimeService);
            dataSource = new Lazy<ITogglDataSource>(CreateDataSource);
            queryFactory = new Lazy<IQueryFactory>(CreateQueryFactory);
            platformInfo = new Lazy<IPlatformInfo>(CreatePlatformInfo);
            ratingService = new Lazy<IRatingService>(CreateRatingService);
            calendarService = new Lazy<ICalendarService>(CreateCalendarService);
            keyValueStorage = new Lazy<IKeyValueStorage>(CreateKeyValueStorage);
            licenseProvider = new Lazy<ILicenseProvider>(CreateLicenseProvider);
            rxActionFactory = new Lazy<IRxActionFactory>(CreateRxActionFactory);
            userPreferences = new Lazy<IUserPreferences>(CreateUserPreferences);
            analyticsService = new Lazy<IAnalyticsService>(CreateAnalyticsService);
            backgroundService = new Lazy<IBackgroundService>(CreateBackgroundService);
            interactorFactory = new Lazy<IInteractorFactory>(CreateInteractorFactory);
            onboardingStorage = new Lazy<IOnboardingStorage>(CreateOnboardingStorage);
            schedulerProvider = new Lazy<ISchedulerProvider>(CreateSchedulerProvider);
            shortcutCreator = new Lazy<IApplicationShortcutCreator>(CreateShortcutCreator);
            notificationService = new Lazy<INotificationService>(CreateNotificationService);
            remoteConfigService = new Lazy<IRemoteConfigService>(CreateRemoteConfigService);
            accessibilityService = new Lazy<IAccessibilityService>(CreateAccessibilityService);
            errorHandlingService = new Lazy<IErrorHandlingService>(CreateErrorHandlingService);
            lastTimeUsageStorage = new Lazy<ILastTimeUsageStorage>(CreateLastTimeUsageStorage);
            backgroundSyncService = new Lazy<IBackgroundSyncService>(CreateBackgroundSyncService);
            automaticSyncingService = new Lazy<IAutomaticSyncingService>(CreateAutomaticSyncingService);
            accessRestrictionStorage = new Lazy<IAccessRestrictionStorage>(CreateAccessRestrictionStorage);
            syncErrorHandlingService = new Lazy<ISyncErrorHandlingService>(CreateSyncErrorHandlingService);
            fetchRemoteConfigService = new Lazy<IFetchRemoteConfigService>(CreateFetchRemoteConfigService);
            remoteConfigUpdateService = new Lazy<IUpdateRemoteConfigCacheService>(CreateUpdateRemoteConfigCacheService);
            privateSharedStorageService = new Lazy<IPrivateSharedStorageService>(CreatePrivateSharedStorageService);
            pushNotificationsTokenService = new Lazy<IPushNotificationsTokenService>(CreatePushNotificationsTokenService);
            pushNotificationsTokenStorage =
                new Lazy<IPushNotificationsTokenStorage>(CreatePushNotificationsTokenStorage);

            api = apiFactory.Select(factory => factory.CreateApiWith(Credentials.None));
            UserAccessManager = new UserAccessManager(
                apiFactory,
                database,
                privateSharedStorageService);

            UserAccessManager
                .UserLoggedIn
                .Subscribe(RecreateLazyDependenciesForLogin)
                .DisposedBy(disposeBag);

            UserAccessManager
                .UserLoggedOut
                .Subscribe(_ => RecreateLazyDependenciesForLogout())
                .DisposedBy(disposeBag);
        }

        protected abstract ITogglDatabase CreateDatabase();
        protected abstract IPlatformInfo CreatePlatformInfo();
        protected abstract IQueryFactory CreateQueryFactory();
        protected abstract IRatingService CreateRatingService();
        protected abstract ICalendarService CreateCalendarService();
        protected abstract IKeyValueStorage CreateKeyValueStorage();
        protected abstract ILicenseProvider CreateLicenseProvider();
        protected abstract IUserPreferences CreateUserPreferences();
        protected abstract IAnalyticsService CreateAnalyticsService();
        protected abstract IOnboardingStorage CreateOnboardingStorage();
        protected abstract ISchedulerProvider CreateSchedulerProvider();
        protected abstract INotificationService CreateNotificationService();
        protected abstract IRemoteConfigService CreateRemoteConfigService();
        protected abstract IAccessibilityService CreateAccessibilityService();
        protected abstract IErrorHandlingService CreateErrorHandlingService();
        protected abstract ILastTimeUsageStorage CreateLastTimeUsageStorage();
        protected abstract IApplicationShortcutCreator CreateShortcutCreator();
        protected abstract IBackgroundSyncService CreateBackgroundSyncService();
        protected abstract IFetchRemoteConfigService CreateFetchRemoteConfigService();
        protected abstract IAccessRestrictionStorage CreateAccessRestrictionStorage();
        protected abstract IPrivateSharedStorageService CreatePrivateSharedStorageService();
        protected abstract IPushNotificationsTokenService CreatePushNotificationsTokenService();

        protected virtual ITimeService CreateTimeService()
            => new TimeService(SchedulerProvider.DefaultScheduler);

        protected virtual IBackgroundService CreateBackgroundService()
            => new BackgroundService(TimeService, AnalyticsService, UpdateRemoteConfigCacheService);

        protected virtual IAutomaticSyncingService CreateAutomaticSyncingService()
            => new AutomaticSyncingService(BackgroundService, TimeService, LastTimeUsageStorage);

        protected virtual ISyncErrorHandlingService CreateSyncErrorHandlingService()
            => new SyncErrorHandlingService(ErrorHandlingService);

        protected virtual ITogglDataSource CreateDataSource()
            => new TogglDataSource(Database, TimeService, AnalyticsService);

        protected virtual IRxActionFactory CreateRxActionFactory()
            => new RxActionFactory(SchedulerProvider);

        protected virtual IApiFactory CreateApiFactory()
            => new ApiFactory(ApiEnvironment, userAgent);

        protected virtual IUpdateRemoteConfigCacheService CreateUpdateRemoteConfigCacheService()
            => new UpdateRemoteConfigCacheService(TimeService, KeyValueStorage, FetchRemoteConfigService);

        protected virtual IPushNotificationsTokenStorage CreatePushNotificationsTokenStorage()
            => new PushNotificationsTokenStorage(KeyValueStorage);

        protected virtual ISyncManager CreateSyncManager()
        {
            var syncManager = TogglSyncManager.CreateSyncManager(
                Database,
                api.Value,
                DataSource,
                TimeService,
                AnalyticsService,
                LastTimeUsageStorage,
                SchedulerProvider.DefaultScheduler,
                AutomaticSyncingService,
                this
            );
            SyncErrorHandlingService.HandleErrorsOf(syncManager);

            return syncManager;
        }

        protected virtual IInteractorFactory CreateInteractorFactory() => new InteractorFactory(
            api.Value,
            UserAccessManager,
            database.Select(database => database.IdProvider),
            database,
            timeService,
            syncManager,
            platformInfo,
            dataSource,
            calendarService,
            userPreferences,
            analyticsService,
            notificationService,
            lastTimeUsageStorage,
            shortcutCreator,
            privateSharedStorageService,
            keyValueStorage,
            pushNotificationsTokenService,
            pushNotificationsTokenStorage
        );

        protected virtual void RecreateLazyDependenciesForLogin(ITogglApi api)
        {
            this.api = new Lazy<ITogglApi>(() => api);

            dataSource = new Lazy<ITogglDataSource>(CreateDataSource);
            syncManager = new Lazy<ISyncManager>(CreateSyncManager);
            interactorFactory = shortcutCreator.Select(creator =>
            {
                var factory = CreateInteractorFactory();
                Task.Run(() => creator.OnLogin(factory));
                return factory;
            });
        }

        protected virtual void RecreateLazyDependenciesForLogout()
        {
            api = apiFactory.Select(factory => factory.CreateApiWith(Credentials.None));

            dataSource = new Lazy<ITogglDataSource>(CreateDataSource);
            syncManager = new Lazy<ISyncManager>(CreateSyncManager);
            interactorFactory = new Lazy<IInteractorFactory>(CreateInteractorFactory);
        }
    }
}
