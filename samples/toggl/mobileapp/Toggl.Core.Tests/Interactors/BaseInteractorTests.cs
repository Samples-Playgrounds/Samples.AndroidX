using NSubstitute;
using System;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Login;
using Toggl.Core.Services;
using Toggl.Core.Shortcuts;
using Toggl.Core.Sync;
using Toggl.Networking;
using Toggl.Networking.Network;
using Toggl.Storage;
using Toggl.Storage.Settings;

namespace Toggl.Core.Tests
{
    public abstract class BaseInteractorTests
    {
        protected IIdProvider IdProvider { get; } = Substitute.For<IIdProvider>();
        protected ITimeService TimeService { get; } = Substitute.For<ITimeService>();
        protected ITogglDataSource DataSource { get; } = Substitute.For<ITogglDataSource>();
        protected ITogglApi Api { get; } = Substitute.For<ITogglApi>();
        protected IUserPreferences UserPreferences { get; } = Substitute.For<IUserPreferences>();
        protected IAnalyticsService AnalyticsService { get; } = Substitute.For<IAnalyticsService>();
        protected IPlatformInfo PlatformInfo { get; } = Substitute.For<IPlatformInfo>();
        protected INotificationService NotificationService { get; } = Substitute.For<INotificationService>();
        protected ILastTimeUsageStorage LastTimeUsageStorage { get; } = Substitute.For<ILastTimeUsageStorage>();
        protected IApplicationShortcutCreator ApplicationShortcutCreator { get; }
            = Substitute.For<IApplicationShortcutCreator>();
        protected UserAgent UserAgent { get; } = new UserAgent("Tests", "0.0");
        protected ICalendarService CalendarService { get; } = Substitute.For<ICalendarService>();
        protected ISyncManager SyncManager { get; } = Substitute.For<ISyncManager>();
        protected ITogglDatabase Database { get; } = Substitute.For<ITogglDatabase>();
        protected IPrivateSharedStorageService PrivateSharedStorageService { get; } =
            Substitute.For<IPrivateSharedStorageService>();
        protected IKeyValueStorage KeyValueStorage { get; } = Substitute.For<IKeyValueStorage>();
        protected IPushNotificationsTokenService PushNotificationsTokenService { get; } =
            Substitute.For<IPushNotificationsTokenService>();
        protected IUserAccessManager UserAccessManager { get; } = Substitute.For<IUserAccessManager>();

        protected IPushNotificationsTokenStorage PushNotificationsTokenStorage { get; } =
            Substitute.For<IPushNotificationsTokenStorage>();

        protected IInteractorFactory InteractorFactory { get; }

        protected BaseInteractorTests()
        {
            InteractorFactory = new InteractorFactory(
                Api,
                UserAccessManager,
                new Lazy<IIdProvider>(() => IdProvider),
                new Lazy<ITogglDatabase>(() => Database),
                new Lazy<ITimeService>(() => TimeService),
                new Lazy<ISyncManager>(() => SyncManager),
                new Lazy<IPlatformInfo>(() => PlatformInfo),
                new Lazy<ITogglDataSource>(() => DataSource),
                new Lazy<ICalendarService>(() => CalendarService),
                new Lazy<IUserPreferences>(() => UserPreferences),
                new Lazy<IAnalyticsService>(() => AnalyticsService),
                new Lazy<INotificationService>(() => NotificationService),
                new Lazy<ILastTimeUsageStorage>(() => LastTimeUsageStorage),
                new Lazy<IApplicationShortcutCreator>(() => ApplicationShortcutCreator),
                new Lazy<IPrivateSharedStorageService>(() => PrivateSharedStorageService),
                new Lazy<IKeyValueStorage>(() => KeyValueStorage),
                new Lazy<IPushNotificationsTokenService>(() => PushNotificationsTokenService),
                new Lazy<IPushNotificationsTokenStorage>(() => PushNotificationsTokenStorage)
            );
        }
    }
}
