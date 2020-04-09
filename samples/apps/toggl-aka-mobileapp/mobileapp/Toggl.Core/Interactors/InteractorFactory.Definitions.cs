using System;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Login;
using Toggl.Core.Reports;
using Toggl.Core.Services;
using Toggl.Core.Shortcuts;
using Toggl.Core.Sync;
using Toggl.Networking;
using Toggl.Shared;
using Toggl.Storage;
using Toggl.Storage.Settings;

namespace Toggl.Core.Interactors
{
    [Preserve(AllMembers = true)]
    public sealed partial class InteractorFactory : IInteractorFactory
    {
        private readonly ITogglApi api;
        private readonly IUserAccessManager userAccessManager;
        private readonly Lazy<IIdProvider> lazyIdProvider;
        private readonly Lazy<ITogglDatabase> lazyDatabase;
        private readonly Lazy<ISyncManager> lazySyncManager;
        private readonly Lazy<ITimeService> lazyTimeService;
        private readonly Lazy<IPlatformInfo> lazyPlatformInfo;
        private readonly Lazy<ITogglDataSource> lazyDataSource;
        private readonly Lazy<IUserPreferences> lazyUserPreferences;
        private readonly Lazy<ICalendarService> lazyCalendarService;
        private readonly Lazy<IAnalyticsService> lazyAnalyticsService;
        private readonly Lazy<INotificationService> lazyNotificationService;
        private readonly Lazy<ILastTimeUsageStorage> lazyLastTimeUsageStorage;
        private readonly Lazy<IApplicationShortcutCreator> lazyShortcutCreator;
        private readonly Lazy<IPrivateSharedStorageService> lazyPrivateSharedStorageService;
        private readonly Lazy<IKeyValueStorage> lazyKeyValueStorage;
        private readonly Lazy<IPushNotificationsTokenService> lazyPushNotificationsTokenService;
        private readonly Lazy<IPushNotificationsTokenStorage> lazyPushNotificationsTokenStorage;
        private readonly ReportsMemoryCache reportsMemoryCache = new ReportsMemoryCache();

        private ITogglDatabase database => lazyDatabase.Value;
        private IIdProvider idProvider => lazyIdProvider.Value;
        private ITimeService timeService => lazyTimeService.Value;
        private ISyncManager syncManager => lazySyncManager.Value;
        private ITogglDataSource dataSource => lazyDataSource.Value;
        private IPlatformInfo platformInfo => lazyPlatformInfo.Value;
        private IUserPreferences userPreferences => lazyUserPreferences.Value;
        private ICalendarService calendarService => lazyCalendarService.Value;
        private IAnalyticsService analyticsService => lazyAnalyticsService.Value;
        private IApplicationShortcutCreator shortcutCreator => lazyShortcutCreator.Value;
        private INotificationService notificationService => lazyNotificationService.Value;
        private ILastTimeUsageStorage lastTimeUsageStorage => lazyLastTimeUsageStorage.Value;
        private IPrivateSharedStorageService privateSharedStorageService => lazyPrivateSharedStorageService.Value;
        private IKeyValueStorage keyValueStorage => lazyKeyValueStorage.Value;
        private IPushNotificationsTokenService pushNotificationsTokenService => lazyPushNotificationsTokenService.Value;

        public InteractorFactory(
            ITogglApi api,
            IUserAccessManager userAccessManager,
            Lazy<IIdProvider> idProvider,
            Lazy<ITogglDatabase> database,
            Lazy<ITimeService> timeService,
            Lazy<ISyncManager> syncManager,
            Lazy<IPlatformInfo> platformInfo,
            Lazy<ITogglDataSource> dataSource,
            Lazy<ICalendarService> calendarService,
            Lazy<IUserPreferences> userPreferences,
            Lazy<IAnalyticsService> analyticsService,
            Lazy<INotificationService> notificationService,
            Lazy<ILastTimeUsageStorage> lastTimeUsageStorage,
            Lazy<IApplicationShortcutCreator> shortcutCreator,
            Lazy<IPrivateSharedStorageService> privateSharedStorageService,
            Lazy<IKeyValueStorage> keyValueStorage,
            Lazy<IPushNotificationsTokenService> pushNotificationsTokenService,
            Lazy<IPushNotificationsTokenStorage> pushNotificationsTokenStorage)
        {
            Ensure.Argument.IsNotNull(api, nameof(api));
            Ensure.Argument.IsNotNull(database, nameof(database));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(idProvider, nameof(idProvider));
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(platformInfo, nameof(platformInfo));
            Ensure.Argument.IsNotNull(calendarService, nameof(calendarService));
            Ensure.Argument.IsNotNull(shortcutCreator, nameof(shortcutCreator));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(userAccessManager, nameof(userAccessManager));
            Ensure.Argument.IsNotNull(notificationService, nameof(notificationService));
            Ensure.Argument.IsNotNull(lastTimeUsageStorage, nameof(lastTimeUsageStorage));
            Ensure.Argument.IsNotNull(privateSharedStorageService, nameof(privateSharedStorageService));
            Ensure.Argument.IsNotNull(keyValueStorage, nameof(keyValueStorage));
            Ensure.Argument.IsNotNull(pushNotificationsTokenService, nameof(pushNotificationsTokenService));
            Ensure.Argument.IsNotNull(pushNotificationsTokenStorage, nameof(pushNotificationsTokenStorage));

            this.api = api;
            this.userAccessManager = userAccessManager;

            lazyDatabase = database;
            lazyDataSource = dataSource;
            lazyIdProvider = idProvider;
            lazySyncManager = syncManager;
            lazyTimeService = timeService;
            lazyPlatformInfo = platformInfo;
            lazyCalendarService = calendarService;
            lazyUserPreferences = userPreferences;
            lazyShortcutCreator = shortcutCreator;
            lazyAnalyticsService = analyticsService;
            lazyNotificationService = notificationService;
            lazyLastTimeUsageStorage = lastTimeUsageStorage;
            lazyPrivateSharedStorageService = privateSharedStorageService;
            lazyKeyValueStorage = keyValueStorage;
            lazyPushNotificationsTokenService = pushNotificationsTokenService;
            lazyPushNotificationsTokenStorage = pushNotificationsTokenStorage;
        }
    }
}
