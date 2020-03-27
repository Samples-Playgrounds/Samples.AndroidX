using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.Login;
using Toggl.Core.Services;
using Toggl.Core.Shortcuts;
using Toggl.Core.Sync;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Toggl.Storage.Settings;

namespace Toggl.Core.Interactors.UserAccess
{
    public sealed class LogoutInteractor : IInteractor<IObservable<Unit>>
    {
        private readonly IAnalyticsService analyticsService;
        private readonly INotificationService notificationService;
        private readonly IApplicationShortcutCreator shortcutCreator;
        private readonly ISyncManager syncManager;
        private readonly ITogglDatabase database;
        private readonly IUserPreferences userPreferences;
        private readonly IPrivateSharedStorageService privateSharedStorageService;
        private readonly IUserAccessManager userAccessManager;
        private readonly IInteractorFactory interactorFactory;
        private readonly LogoutSource source;

        public LogoutInteractor(
            IAnalyticsService analyticsService,
            INotificationService notificationService,
            IApplicationShortcutCreator shortcutCreator,
            ISyncManager syncManager,
            ITogglDatabase database,
            IUserPreferences userPreferences,
            IPrivateSharedStorageService privateSharedStorageService,
            IUserAccessManager userAccessManager,
            IInteractorFactory interactorFactory,
            LogoutSource source)
        {
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(notificationService, nameof(notificationService));
            Ensure.Argument.IsNotNull(shortcutCreator, nameof(shortcutCreator));
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));
            Ensure.Argument.IsNotNull(database, nameof(database));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(privateSharedStorageService, nameof(privateSharedStorageService));
            Ensure.Argument.IsNotNull(userAccessManager, nameof(userAccessManager));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsADefinedEnumValue(source, nameof(source));

            this.analyticsService = analyticsService;
            this.shortcutCreator = shortcutCreator;
            this.notificationService = notificationService;
            this.syncManager = syncManager;
            this.database = database;
            this.userPreferences = userPreferences;
            this.privateSharedStorageService = privateSharedStorageService;
            this.userAccessManager = userAccessManager;
            this.interactorFactory = interactorFactory;
            this.source = source;
        }

        public IObservable<Unit> Execute()
            => syncManager.Freeze()
                .FirstAsync()
                .SelectMany(_ => database.Clear())
                .Do(shortcutCreator.OnLogout)
                .Do(userPreferences.Reset)
                .Do(privateSharedStorageService.ClearAll)
                .Do(_ => analyticsService.Logout.Track(source))
                .SelectMany(_ =>
                    notificationService
                        .UnscheduleAllNotifications()
                        .Catch(Observable.Return(Unit.Default)))
                .SelectMany(interactorFactory.UnsubscribeFromPushNotifications().Execute())
                .Do(userAccessManager.OnUserLoggedOut)
                .Do(analyticsService.ResetAppCenterUserId)
                .FirstAsync();
    }
}
