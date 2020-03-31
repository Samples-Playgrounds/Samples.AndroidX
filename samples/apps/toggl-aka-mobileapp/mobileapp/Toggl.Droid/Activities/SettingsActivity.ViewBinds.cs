using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Widget;
using Google.Android.Material.AppBar;
using Toggl.Core.Sync;
using Toggl.Droid.Extensions;
using static Toggl.Core.Sync.PresentableSyncStatus;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Toggl.Droid.Activities
{
    public partial class SettingsActivity
    {
        private View aboutView;
        private View swipeActionsView;
        private View manualModeView;
        private View is24hoursModeView;
        private View runningTimerNotificationsView;
        private View stoppedTimerNotificationsView;
        private View dateFormatView;
        private View beginningOfWeekView;
        private View durationFormatView;
        private View smartRemindersView;
        private View smartRemindersViewSeparator;
        private View groupTimeEntriesView;
        private View defaultWorkspaceView;
        private View finalSeparator;

        private TextView helpView;
        private TextView calendarSettingsView;
        private TextView nameTextView;
        private TextView emailTextView;
        private TextView versionTextView;
        private TextView dateFormatTextView;
        private TextView beginningOfWeekTextView;
        private TextView durationFormatTextView;
        private TextView smartRemindersTextView;
        private TextView defaultWorkspaceNameTextView;
        private TextView yourProfileLabel;
        private TextView usernameLabel;
        private TextView emailLabel;
        private TextView defaultWorkspaceLabel;
        private TextView displayLabel;
        private TextView dateFormatLabel;
        private TextView beginningOfWeekLabel;
        private TextView durationFormatLabel;
        private TextView use24HourClockLabel;
        private TextView groupedTimeEntriesLabel;
        private TextView smartRemindersLabel;
        private TextView notificationsLabel;
        private TextView notificationsRunningTimerLabel;
        private TextView notificationsStoppedTimerLabel;
        private TextView generalLabel;
        private TextView aboutLabel;
        private TextView settingsToggleSwipeActionsLabel;
        private TextView settingsToggleManualModeLabel;
        private TextView settingsToggleManualModeExplanation;
        private TextView feedbackView;
        private View logoutView;
        private TextView txtLogout;

        private Switch is24hoursModeSwitch;
        private Switch swipeActionsSwitch;
        private Switch manualModeSwitch;
        private Switch runningTimerNotificationsSwitch;
        private Switch stoppedTimerNotificationsSwitch;
        private Switch groupTimeEntriesSwitch;

        private AppBarLayout appBarLayout;
        private NestedScrollView scrollView;

        private View syncStateSyncedHolder;
        private View syncStateInProgressHolder;
        private Dictionary<PresentableSyncStatus, View> syncStateViews = new Dictionary<PresentableSyncStatus, View>();
        private TextView txtStateSynced;
        private TextView txtStateInProgress;

        private Toolbar toolbar;

        protected override void InitializeViews()
        {
            finalSeparator = FindViewById(Resource.Id.FinalSeparator);
            aboutView = FindViewById(Resource.Id.SettingsAboutContainer);
            swipeActionsView = FindViewById(Resource.Id.SettingsToggleSwipeActionsView);
            manualModeView = FindViewById(Resource.Id.SettingsToggleManualModeView);
            is24hoursModeView = FindViewById(Resource.Id.SettingsIs24HourModeView);
            dateFormatView = FindViewById(Resource.Id.SettingsDateFormatView);
            beginningOfWeekView = FindViewById(Resource.Id.SettingsSelectBeginningOfWeekView);
            durationFormatView = FindViewById(Resource.Id.SettingsDurationFormatView);
            smartRemindersView = FindViewById(Resource.Id.SmartRemindersView);
            smartRemindersViewSeparator = FindViewById(Resource.Id.SmartRemindersViewSeparator);
            runningTimerNotificationsView = FindViewById(Resource.Id.SettingsRunningTimerNotificationsView);
            stoppedTimerNotificationsView = FindViewById(Resource.Id.SettingsStoppedTimerNotificationsView);
            groupTimeEntriesView = FindViewById(Resource.Id.GroupTimeEntriesView);
            defaultWorkspaceView = FindViewById(Resource.Id.DefaultWorkspaceView);
      
            txtLogout = FindViewById<TextView>(Resource.Id.TxtSettingsLogout);
            logoutView = FindViewById(Resource.Id.SettingsLogoutButton);
            helpView = FindViewById<TextView>(Resource.Id.SettingsHelpButton);
            feedbackView = FindViewById<TextView>(Resource.Id.SettingsSubmitFeedbackButton);
            calendarSettingsView = FindViewById<TextView>(Resource.Id.CalendarSettingsView);
            nameTextView = FindViewById<TextView>(Resource.Id.SettingsNameTextView);
            emailTextView = FindViewById<TextView>(Resource.Id.SettingsEmailTextView);
            versionTextView = FindViewById<TextView>(Resource.Id.SettingsAppVersionTextView);
            dateFormatTextView = FindViewById<TextView>(Resource.Id.SettingsDateFormatTextView);
            beginningOfWeekTextView = FindViewById<TextView>(Resource.Id.SettingsBeginningOfWeekTextView);
            durationFormatTextView = FindViewById<TextView>(Resource.Id.SettingsDurationFormatTextView);
            smartRemindersTextView = FindViewById<TextView>(Resource.Id.SmartRemindersTextView);
            defaultWorkspaceNameTextView = FindViewById<TextView>(Resource.Id.DefaultWorkspaceName);
            yourProfileLabel = FindViewById<TextView>(Resource.Id.YourProfileLabel);
            usernameLabel = FindViewById<TextView>(Resource.Id.UsernameLabel);
            emailLabel = FindViewById<TextView>(Resource.Id.EmailLabel);
            defaultWorkspaceLabel = FindViewById<TextView>(Resource.Id.DefaultWorkspaceLabel);
            displayLabel = FindViewById<TextView>(Resource.Id.DisplayLabel);
            dateFormatLabel = FindViewById<TextView>(Resource.Id.DateFormatLabel);
            beginningOfWeekLabel = FindViewById<TextView>(Resource.Id.BeginningOfWeekLabel);
            durationFormatLabel = FindViewById<TextView>(Resource.Id.DurationFormatLabel);
            use24HourClockLabel = FindViewById<TextView>(Resource.Id.Use24HourClockLabel);
            groupedTimeEntriesLabel = FindViewById<TextView>(Resource.Id.GroupedTimeEntriesLabel);
            smartRemindersLabel = FindViewById<TextView>(Resource.Id.SmartRemindersLabel);
            notificationsLabel = FindViewById<TextView>(Resource.Id.NotificationsLabel);
            notificationsRunningTimerLabel = FindViewById<TextView>(Resource.Id.NotificationsRunningTimerLabel);
            notificationsStoppedTimerLabel = FindViewById<TextView>(Resource.Id.NotificationsStoppedTimerLabel);
            generalLabel = FindViewById<TextView>(Resource.Id.GeneralLabel);
            aboutLabel = FindViewById<TextView>(Resource.Id.AboutLabel);
            settingsToggleSwipeActionsLabel = FindViewById<TextView>(Resource.Id.SettingsToggleSwipeActionsLabel);
            settingsToggleManualModeLabel = FindViewById<TextView>(Resource.Id.SettingsToggleManualModeLabel);
            settingsToggleManualModeExplanation = FindViewById<TextView>(Resource.Id.SettingsToggleManualModeExplanation);

            swipeActionsSwitch = FindViewById<Switch>(Resource.Id.SettingsAreSwipeActionsEnabledSwitch);
            manualModeSwitch = FindViewById<Switch>(Resource.Id.SettingsIsManualModeEnabledSwitch);
            is24hoursModeSwitch = FindViewById<Switch>(Resource.Id.SettingsIs24HourModeSwitch);
            runningTimerNotificationsSwitch = FindViewById<Switch>(Resource.Id.SettingsAreRunningTimerNotificationsEnabledSwitch);
            stoppedTimerNotificationsSwitch = FindViewById<Switch>(Resource.Id.SettingsAreStoppedTimerNotificationsEnabledSwitch);
            groupTimeEntriesSwitch = FindViewById<Switch>(Resource.Id.GroupTimeEntriesSwitch);

            appBarLayout = FindViewById<AppBarLayout>(Resource.Id.AppBarLayout);

            scrollView = FindViewById<NestedScrollView>(Resource.Id.ScrollView);
            
            syncStateSyncedHolder = FindViewById(Resource.Id.SyncStateSyncedHolder);
            syncStateInProgressHolder = FindViewById(Resource.Id.SyncStateInProgressHolder);
            syncStateViews.Add(Synced, syncStateSyncedHolder);
            syncStateViews.Add(Syncing, syncStateInProgressHolder);
            syncStateViews.Add(LoggingOut, syncStateInProgressHolder);
            txtStateSynced = FindViewById<TextView>(Resource.Id.TxtStateSynced);
            txtStateInProgress = FindViewById<TextView>(Resource.Id.TxtStateInProgress);
            
            txtLogout.Text = Shared.Resources.SignOutOfToggl;
            helpView.Text = Shared.Resources.Help;
            feedbackView.Text = Shared.Resources.SubmitFeedback;
            calendarSettingsView.Text = Shared.Resources.CalendarSettingsTitle;
            yourProfileLabel.Text = Shared.Resources.YourProfile;
            usernameLabel.Text = Shared.Resources.Username;
            emailLabel.Text = Shared.Resources.Email;
            defaultWorkspaceLabel.Text = Shared.Resources.DefaultWorkspace;
            displayLabel.Text = Shared.Resources.Display;
            dateFormatLabel.Text = Shared.Resources.DateFormat;
            beginningOfWeekLabel.Text = Shared.Resources.FirstDayOfTheWeek;
            durationFormatLabel.Text = Shared.Resources.DurationFormat;
            use24HourClockLabel.Text = Shared.Resources.Use24HourClock;
            groupedTimeEntriesLabel.Text = Shared.Resources.GroupTimeEntries;
            smartRemindersLabel.Text = Shared.Resources.SmartReminders;
            notificationsLabel.Text = Shared.Resources.Notifications;
            notificationsRunningTimerLabel.Text = Shared.Resources.NotificationsRunningTimer;
            notificationsStoppedTimerLabel.Text = Shared.Resources.NotificationsStoppedTimer;
            generalLabel.Text = Shared.Resources.General;
            aboutLabel.Text = Shared.Resources.About;
            settingsToggleSwipeActionsLabel.Text = Shared.Resources.SwipeActions;
            settingsToggleManualModeLabel.Text = Shared.Resources.ManualMode;
            settingsToggleManualModeExplanation.Text = Shared.Resources.ManualModeDescription;
            txtStateSynced.Text = Shared.Resources.SyncCompleted;
            txtStateInProgress.Text = Shared.Resources.Syncing;

            toolbar = FindViewById<Toolbar>(Resource.Id.Toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = Shared.Resources.Settings;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            var baseMargin = 24.DpToPixels(this);
            finalSeparator.DoOnApplyWindowInsets((v, insets, initialPadding) =>
            {
                var bottomMargin = baseMargin + insets.SystemWindowInsetBottom;
                var currentLayoutParams = finalSeparator.LayoutParameters as LinearLayout.LayoutParams;
                finalSeparator.LayoutParameters = currentLayoutParams.WithMargins(bottom: bottomMargin);
            });
        }
    }
}
