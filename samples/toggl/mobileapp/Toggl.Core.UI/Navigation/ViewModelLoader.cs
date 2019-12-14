using System;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Core.UI.ViewModels.Settings.Siri;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.Navigation
{
    public sealed class ViewModelLoader
    {
        private readonly UIDependencyContainer dependencyContainer;

        public ViewModelLoader(UIDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        public TViewModel Load<TViewModel>()
            where TViewModel : IViewModel
            => (TViewModel)findViewModel(typeof(TViewModel));

        private IViewModel findViewModel(Type viewModelType)
        {
            if (viewModelType == typeof(EditDurationViewModel))
            {
                return new EditDurationViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.TimeService,
                    dependencyContainer.DataSource,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.SchedulerProvider);
            }

            if (viewModelType == typeof(EditProjectViewModel))
            {
                return new EditProjectViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.NavigationService);
            }

            if (viewModelType == typeof(EditTimeEntryViewModel))
            {
                return new EditTimeEntryViewModel(
                    dependencyContainer.TimeService,
                    dependencyContainer.DataSource,
                    dependencyContainer.SyncManager,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.SchedulerProvider);
            }

            if (viewModelType == typeof(ForgotPasswordViewModel))
            {
                return new ForgotPasswordViewModel(
                    dependencyContainer.TimeService,
                    dependencyContainer.UserAccessManager,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.NavigationService,
                    dependencyContainer.RxActionFactory);
            }

            if (viewModelType == typeof(LoginViewModel))
            {
                return new LoginViewModel(
                    dependencyContainer.UserAccessManager,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.NavigationService,
                    dependencyContainer.ErrorHandlingService,
                    dependencyContainer.LastTimeUsageStorage,
                    dependencyContainer.TimeService,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.InteractorFactory);
            }

            if (viewModelType == typeof(MainTabBarViewModel))
            {
                return new MainTabBarViewModel(
                    dependencyContainer.TimeService,
                    dependencyContainer.DataSource,
                    dependencyContainer.SyncManager,
                    dependencyContainer.RatingService,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.BackgroundService,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.PermissionsChecker,
                    dependencyContainer.NavigationService,
                    dependencyContainer.RemoteConfigService,
                    dependencyContainer.AccessibilityService,
                    dependencyContainer.UpdateRemoteConfigCacheService,
                    dependencyContainer.AccessRestrictionStorage,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.UserAccessManager,
                    dependencyContainer.PrivateSharedStorageService,
                    dependencyContainer.PlatformInfo,
                    dependencyContainer.WidgetsService,
                    dependencyContainer.LastTimeUsageStorage);
            }

            if (viewModelType == typeof(MainViewModel))
            {
                return new MainViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.SyncManager,
                    dependencyContainer.TimeService,
                    dependencyContainer.RatingService,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.RemoteConfigService,
                    dependencyContainer.AccessibilityService,
                    dependencyContainer.UpdateRemoteConfigCacheService,
                    dependencyContainer.AccessRestrictionStorage,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.PermissionsChecker,
                    dependencyContainer.BackgroundService,
                    dependencyContainer.PlatformInfo,
                    dependencyContainer.WidgetsService,
                    dependencyContainer.LastTimeUsageStorage);
            }

            if (viewModelType == typeof(NoWorkspaceViewModel))
            {
                return new NoWorkspaceViewModel(
                    dependencyContainer.SyncManager,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.AccessRestrictionStorage,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory);
            }

            if (viewModelType == typeof(OutdatedAppViewModel))
            {
                return new OutdatedAppViewModel(
                    dependencyContainer.PlatformInfo,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.NavigationService);
            }

            if (viewModelType == typeof(RatingViewModel))
            {
                return new RatingViewModel(
                    dependencyContainer.TimeService,
                    dependencyContainer.RatingService,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.NavigationService,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory);
            }

            if (viewModelType == typeof(ReportsCalendarViewModel))
            {
                return new ReportsCalendarViewModel(
                    dependencyContainer.TimeService,
                    dependencyContainer.DataSource,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.SchedulerProvider);
            }

            if (viewModelType == typeof(SelectClientViewModel))
            {
                return new SelectClientViewModel(
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory);
            }

            if (viewModelType == typeof(SelectColorViewModel))
            {
                return new SelectColorViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.SchedulerProvider);
            }

            if (viewModelType == typeof(SelectCountryViewModel))
            {
                return new SelectCountryViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.RxActionFactory);
            }

            if (viewModelType == typeof(SelectDateTimeViewModel))
            {
                return new SelectDateTimeViewModel(
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.NavigationService);
            }

            if (viewModelType == typeof(SelectDefaultWorkspaceViewModel))
            {
                return new SelectDefaultWorkspaceViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.AccessRestrictionStorage,
                    dependencyContainer.RxActionFactory);
            }

            if (viewModelType == typeof(SelectProjectViewModel))
            {
                return new SelectProjectViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.SchedulerProvider);
            }

            if (viewModelType == typeof(SelectTagsViewModel))
            {
                return new SelectTagsViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory);
            }

            if (viewModelType == typeof(SignupViewModel))
            {
                return new SignupViewModel(
                    dependencyContainer.ApiFactory,
                    dependencyContainer.UserAccessManager,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.NavigationService,
                    dependencyContainer.ErrorHandlingService,
                    dependencyContainer.LastTimeUsageStorage,
                    dependencyContainer.TimeService,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.PlatformInfo);
            }

            if (viewModelType == typeof(StartTimeEntryViewModel))
            {
                return new StartTimeEntryViewModel(
                    dependencyContainer.TimeService,
                    dependencyContainer.DataSource,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory);
            }

            if (viewModelType == typeof(SuggestionsViewModel))
            {
                return new SuggestionsViewModel(
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.TimeService,
                    dependencyContainer.PermissionsChecker,
                    dependencyContainer.NavigationService,
                    dependencyContainer.BackgroundService,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.SyncManager,
                    dependencyContainer.WidgetsService);
            }

            if (viewModelType == typeof(SyncFailuresViewModel))
            {
                return new SyncFailuresViewModel(
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService);
            }

            if (viewModelType == typeof(TermsOfServiceViewModel))
            {
                return new TermsOfServiceViewModel(
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.NavigationService);
            }

            if (viewModelType == typeof(TokenResetViewModel))
            {
                return new TokenResetViewModel(
                    dependencyContainer.UserAccessManager,
                    dependencyContainer.DataSource,
                    dependencyContainer.NavigationService,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.InteractorFactory);
            }

            if (viewModelType == typeof(CalendarPermissionDeniedViewModel))
            {
                return new CalendarPermissionDeniedViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.PermissionsChecker,
                    dependencyContainer.RxActionFactory);
            }

            if (viewModelType == typeof(CalendarViewModel))
            {
                return new CalendarViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.TimeService,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.BackgroundService,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.NavigationService);
            }

            if (viewModelType == typeof(ReportsViewModel))
            {
                return new ReportsViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.TimeService,
                    dependencyContainer.NavigationService,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory);
            }

            if (viewModelType == typeof(AboutViewModel))
            {
                return new AboutViewModel(
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.NavigationService);
            }

            if (viewModelType == typeof(CalendarSettingsViewModel))
            {
                return new CalendarSettingsViewModel(
                    dependencyContainer.UserPreferences,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.NavigationService,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.PermissionsChecker,
                    dependencyContainer.SchedulerProvider);
            }

            if (viewModelType == typeof(LicensesViewModel))
            {
                return new LicensesViewModel(
                    dependencyContainer.LicenseProvider,
                    dependencyContainer.NavigationService);
            }

            if (viewModelType == typeof(NotificationSettingsViewModel))
            {
                return new NotificationSettingsViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.BackgroundService,
                    dependencyContainer.PermissionsChecker,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory);
            }

            if (viewModelType == typeof(SendFeedbackViewModel))
            {
                return new SendFeedbackViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory);
            }

            if (viewModelType == typeof(SettingsViewModel))
            {
                return new SettingsViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.SyncManager,
                    dependencyContainer.PlatformInfo,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.PermissionsChecker,
                    dependencyContainer.SchedulerProvider);
            }

            if (viewModelType == typeof(UpcomingEventsNotificationSettingsViewModel))
            {
                return new UpcomingEventsNotificationSettingsViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.RxActionFactory);
            }

            if (viewModelType == typeof(SiriShortcutsSelectReportPeriodViewModel))
            {
                return new SiriShortcutsSelectReportPeriodViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.NavigationService);
            }

            if (viewModelType == typeof(SiriShortcutsViewModel))
            {
                return new SiriShortcutsViewModel(
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.NavigationService);
            }

            if (viewModelType == typeof(SiriWorkflowsViewModel))
            {
                return new SiriWorkflowsViewModel(dependencyContainer.NavigationService);
            }

            if (viewModelType == typeof(SiriShortcutsCustomTimeEntryViewModel))
            {
                return new SiriShortcutsCustomTimeEntryViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.NavigationService);
            }

            if (viewModelType == typeof(PasteFromClipboardViewModel))
            {
                return new PasteFromClipboardViewModel(
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.NavigationService);
            }

            if (viewModelType == typeof(January2020CampaignViewModel))
            {
                return new January2020CampaignViewModel(
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.RemoteConfigService,
                    dependencyContainer.PlatformInfo,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.NavigationService);
            }

            throw new InvalidOperationException($"Trying to locate ViewModel {viewModelType.Name} failed.");
        }
    }
}
