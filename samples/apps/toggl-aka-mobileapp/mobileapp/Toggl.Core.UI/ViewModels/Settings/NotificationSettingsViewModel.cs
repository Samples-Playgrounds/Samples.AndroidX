using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Extensions;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels.Settings
{
    [Preserve(AllMembers = true)]
    public sealed class NotificationSettingsViewModel : ViewModel
    {
        public IObservable<bool> PermissionGranted { get; }
        public IObservable<string> UpcomingEvents { get; }

        public ViewAction RequestAccess { get; }
        public ViewAction OpenUpcomingEvents { get; }

        public NotificationSettingsViewModel(
            INavigationService navigationService,
            IBackgroundService backgroundService,
            IPermissionsChecker permissionsChecker,
            IUserPreferences userPreferences,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(backgroundService, nameof(backgroundService));
            Ensure.Argument.IsNotNull(permissionsChecker, nameof(permissionsChecker));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            PermissionGranted = backgroundService.AppResumedFromBackground
                .SelectUnit()
                .StartWith(Unit.Default)
                .SelectMany(_ => permissionsChecker.NotificationPermissionGranted)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            UpcomingEvents = userPreferences.CalendarNotificationsSettings()
                .Select(s => s.Title())
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            RequestAccess = rxActionFactory.FromAction(requestAccess);
            OpenUpcomingEvents = rxActionFactory.FromAsync(openUpcomingEvents);
        }

        private void requestAccess()
        {
            View.OpenAppSettings();
        }

        private async Task openUpcomingEvents()
        {
            await Navigate<UpcomingEventsNotificationSettingsViewModel, Unit>();
        }
    }
}
