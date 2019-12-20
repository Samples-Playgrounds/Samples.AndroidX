using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Interactors;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels.Settings
{
    [Preserve(AllMembers = true)]
    public sealed class CalendarSettingsViewModel : SelectUserCalendarsViewModelBase
    {
        private readonly IPermissionsChecker permissionsChecker;
        private readonly IRxActionFactory rxActionFactory;

        private BehaviorSubject<bool> calendarListVisibleSubject = new BehaviorSubject<bool>(false);
        private BehaviorSubject<bool> permissionGrantedSubject = new BehaviorSubject<bool>(false);

        public IObservable<bool> PermissionGranted { get; }
        public IObservable<bool> CalendarListVisible { get; }

        public ViewAction RequestAccess { get; }
        public ViewAction RequestCalendarPermissionsIfNeeded { get; }
        public ViewAction TogglCalendarIntegration { get; }

        public CalendarSettingsViewModel(
            IUserPreferences userPreferences,
            IInteractorFactory interactorFactory,
            IOnboardingStorage onboardingStorage,
            IAnalyticsService analyticsService,
            INavigationService navigationService,
            IRxActionFactory rxActionFactory,
            IPermissionsChecker permissionsChecker,
            ISchedulerProvider schedulerProvider)
            : base(userPreferences, interactorFactory, onboardingStorage, analyticsService, navigationService, rxActionFactory)
        {
            Ensure.Argument.IsNotNull(permissionsChecker, nameof(permissionsChecker));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));

            this.permissionsChecker = permissionsChecker;

            RequestAccess = rxActionFactory.FromAction(requestAccess);
            TogglCalendarIntegration = rxActionFactory.FromAsync(toggleCalendarIntegration);
            RequestCalendarPermissionsIfNeeded = rxActionFactory.FromAsync(requestCalendarPermissionsIfNeeded);

            CalendarListVisible = calendarListVisibleSubject.AsObservable();
            PermissionGranted = permissionGrantedSubject.AsObservable().DistinctUntilChanged().AsDriver(schedulerProvider);
        }

        public override async Task Initialize(bool forceItemSelection)
        {
            permissionGrantedSubject.OnNext(await permissionsChecker.CalendarPermissionGranted);

            if (!permissionGrantedSubject.Value)
            {
                UserPreferences.SetEnabledCalendars();
            }

            await base.Initialize(forceItemSelection);

            var calendarListVisible = permissionGrantedSubject.Value;
            calendarListVisibleSubject.OnNext(calendarListVisible);
        }

        public override Task<bool> CloseWithDefaultResult()
        {
            UserPreferences.SetEnabledCalendars(InitialSelectedCalendarIds.ToArray());
            return base.CloseWithDefaultResult();
        }

        protected override void Done()
        {
            if (!calendarListVisibleSubject.Value)
                SelectedCalendarIds.Clear();

            UserPreferences.SetEnabledCalendars(SelectedCalendarIds.ToArray());
            base.Done();
        }

        private void requestAccess()
        {
            View.OpenAppSettings();
        }

        private async Task requestCalendarPermissionsIfNeeded()
        {
            var authorized = await permissionsChecker.CalendarPermissionGranted;
            if (!authorized)
            {
                authorized = await View.RequestCalendarAuthorization();
                if (!authorized)
                    await Navigate<CalendarPermissionDeniedViewModel, Unit>();

                ReloadCalendars();
                permissionGrantedSubject.OnNext(authorized);
                calendarListVisibleSubject.OnNext(authorized);
            }
        }

        private async Task toggleCalendarIntegration()
        {
            var calendarListVisible = calendarListVisibleSubject.Value;

            if (calendarListVisible)
            {
                calendarListVisible = false;
            }
            else
            {
                await requestCalendarPermissionsIfNeeded();
                var authorized = await permissionsChecker.CalendarPermissionGranted;
                if (authorized)
                {
                    calendarListVisible = true;
                }
            }
            calendarListVisibleSubject.OnNext(calendarListVisible);
        }
    }
}
