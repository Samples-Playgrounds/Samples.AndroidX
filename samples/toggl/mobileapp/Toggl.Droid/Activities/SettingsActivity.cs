using Android.Views;
using Android.Widget;
using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Toggl.Core.Sync;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Shared.Extensions;
using static Toggl.Shared.Resources;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public partial class SettingsActivity : ReactiveActivity<SettingsViewModel>
    {
        public SettingsActivity() : base(
            Resource.Layout.SettingsActivity,
            Resource.Style.AppTheme,
            Transitions.SlideInFromBottom)
        {
        }

        public SettingsActivity(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override void InitializeBindings()
        {
            scrollView.AttachMaterialScrollBehaviour(appBarLayout);

            versionTextView.Text = ViewModel.Version;

            ViewModel.Name
                .Subscribe(nameTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.Email
                .Subscribe(emailTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.WorkspaceName
                .Subscribe(defaultWorkspaceNameTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.SwipeActionsEnabled
                .Subscribe(swipeActionsSwitch.Rx().CheckedObserver(ignoreUnchanged: true))
                .DisposedBy(DisposeBag);

            ViewModel.IsManualModeEnabled
                .Subscribe(manualModeSwitch.Rx().CheckedObserver(ignoreUnchanged: true))
                .DisposedBy(DisposeBag);

            ViewModel.IsGroupingTimeEntries
               .Subscribe(groupTimeEntriesSwitch.Rx().CheckedObserver(ignoreUnchanged: true))
               .DisposedBy(DisposeBag);

            ViewModel.UseTwentyFourHourFormat
                .Subscribe(is24hoursModeSwitch.Rx().CheckedObserver(ignoreUnchanged: true))
                .DisposedBy(DisposeBag);

            ViewModel.AreRunningTimerNotificationsEnabled
                .Subscribe(runningTimerNotificationsSwitch.Rx().CheckedObserver(ignoreUnchanged: true))
                .DisposedBy(DisposeBag);

            ViewModel.AreStoppedTimerNotificationsEnabled
                .Subscribe(stoppedTimerNotificationsSwitch.Rx().CheckedObserver(ignoreUnchanged: true))
                .DisposedBy(DisposeBag);

            ViewModel.DateFormat
                .Subscribe(dateFormatTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.BeginningOfWeek
                .Subscribe(beginningOfWeekTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.DurationFormat
                .Subscribe(durationFormatTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.IsCalendarSmartRemindersVisible
                .Subscribe(smartRemindersView.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsCalendarSmartRemindersVisible
                .Subscribe(smartRemindersViewSeparator.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.CalendarSmartReminders
                .Subscribe(smartRemindersTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.LoggingOut
                .Subscribe(this.CancelAllNotifications)
                .DisposedBy(DisposeBag);

            ViewModel.IsFeedbackSuccessViewShowing
                .Subscribe(showFeedbackSuccessToast)
                .DisposedBy(DisposeBag);

            logoutView.Rx()
                .BindAction(ViewModel.TryLogout)
                .DisposedBy(DisposeBag);

            helpView.Rx()
                .BindAction(ViewModel.OpenHelpView)
                .DisposedBy(DisposeBag);

            aboutView.Rx()
                .BindAction(ViewModel.OpenAboutView)
                .DisposedBy(DisposeBag);

            defaultWorkspaceView.Rx()
                .BindAction(ViewModel.PickDefaultWorkspace)
                .DisposedBy(DisposeBag);

            feedbackView.Rx()
                .BindAction(ViewModel.SubmitFeedback)
                .DisposedBy(DisposeBag);

            swipeActionsView.Rx()
                .BindAction(ViewModel.ToggleSwipeActions)
                .DisposedBy(DisposeBag);

            swipeActionsSwitch.Rx()
                .BindAction(ViewModel.ToggleSwipeActions)
                .DisposedBy(DisposeBag);

            manualModeView.Rx()
                .BindAction(ViewModel.ToggleManualMode)
                .DisposedBy(DisposeBag);

            manualModeSwitch.Rx()
                .BindAction(ViewModel.ToggleManualMode)
                .DisposedBy(DisposeBag);

            groupTimeEntriesView.Rx()
                .BindAction(ViewModel.ToggleTimeEntriesGrouping)
                .DisposedBy(DisposeBag);

            groupTimeEntriesSwitch.Rx()
                .BindAction(ViewModel.ToggleTimeEntriesGrouping)
                .DisposedBy(DisposeBag);

            is24hoursModeView.Rx()
                .BindAction(ViewModel.ToggleTwentyFourHourSettings)
                .DisposedBy(DisposeBag);

            is24hoursModeSwitch.Rx()
                .BindAction(ViewModel.ToggleTwentyFourHourSettings)
                .DisposedBy(DisposeBag);

            runningTimerNotificationsView.Rx().Tap()
                .Subscribe(ViewModel.ToggleRunningTimerNotifications)
                .DisposedBy(DisposeBag);

            runningTimerNotificationsSwitch.Rx().Tap()
                .Subscribe(ViewModel.ToggleRunningTimerNotifications)
                .DisposedBy(DisposeBag);

            stoppedTimerNotificationsView.Rx().Tap()
                .Subscribe(ViewModel.ToggleStoppedTimerNotifications)
                .DisposedBy(DisposeBag);

            stoppedTimerNotificationsSwitch.Rx().Tap()
                .Subscribe(ViewModel.ToggleStoppedTimerNotifications)
                .DisposedBy(DisposeBag);

            dateFormatView.Rx().Tap()
                .Subscribe(ViewModel.SelectDateFormat.Inputs)
                .DisposedBy(DisposeBag);

            beginningOfWeekView.Rx()
                .BindAction(ViewModel.SelectBeginningOfWeek)
                .DisposedBy(DisposeBag);

            durationFormatView.Rx().Tap()
                .Subscribe(ViewModel.SelectDurationFormat.Inputs)
                .DisposedBy(DisposeBag);

            calendarSettingsView.Rx().Tap()
                .Subscribe(ViewModel.OpenCalendarSettings.Inputs)
                .DisposedBy(DisposeBag);

            smartRemindersView.Rx().Tap()
                .Subscribe(ViewModel.OpenCalendarSmartReminders.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.CurrentSyncStatus
                .Subscribe(setSyncStatusView)
                .DisposedBy(DisposeBag);
        }

        private void setSyncStatusView(PresentableSyncStatus status)
        {
            syncStateViews.Values.ForEach(view => view.Visibility = ViewStates.Gone);

            txtStateInProgress.Text = status == PresentableSyncStatus.Syncing ? Syncing : LoggingOutSecurely;

            var visibleView = syncStateViews[status];
            visibleView.Visibility = ViewStates.Visible;
        }

        private void showFeedbackSuccessToast(bool succeeded)
        {
            if (!succeeded) return;

            var toast = Toast.MakeText(this, Shared.Resources.SendFeedbackSuccessMessage, ToastLength.Long);
            toast.SetGravity(GravityFlags.CenterHorizontal | GravityFlags.Bottom, 0, 0);
            toast.Show();
        }
    }
}
