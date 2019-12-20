using System;
using System.Collections.Generic;
using System.Diagnostics;
using Accord.Statistics.Kernels;
using Toggl.Core.Extensions;
using Toggl.Core.Suggestions;
using Toggl.Core.Sync;
using Toggl.Shared;

namespace Toggl.Core.Analytics
{
    [Preserve(AllMembers = true)]
    public abstract class BaseAnalyticsService : IAnalyticsService
    {
        public IAnalyticsEvent<AuthenticationMethod> Login { get; }

        public IAnalyticsEvent<LoginErrorSource> LoginError { get; }

        public IAnalyticsEvent<AuthenticationMethod> SignUp { get; }

        public IAnalyticsEvent<SignUpErrorSource> SignUpError { get; }

        public IAnalyticsEvent<LoginSignupAuthenticationMethod> UserIsMissingApiToken { get; }

        public IAnalyticsEvent<string> OnboardingSkip { get; }

        public IAnalyticsEvent<LogoutSource> Logout { get; }

        public IAnalyticsEvent ResetPassword { get; }

        public IAnalyticsEvent PasswordManagerButtonClicked { get; }

        public IAnalyticsEvent PasswordManagerContainsValidEmail { get; }

        public IAnalyticsEvent PasswordManagerContainsValidPassword { get; }

        public IAnalyticsEvent<Type> CurrentPage { get; }

        public IAnalyticsEvent<DeleteTimeEntryOrigin> DeleteTimeEntry { get; }

        public IAnalyticsEvent<string> ApplicationShortcut { get; }

        public IAnalyticsEvent EditEntrySelectProject { get; }

        public IAnalyticsEvent EditEntrySelectTag { get; }

        public IAnalyticsEvent<ProjectTagSuggestionSource> StartEntrySelectProject { get; }

        public IAnalyticsEvent<ProjectTagSuggestionSource> StartEntrySelectTag { get; }

        public IAnalyticsEvent RatingViewWasShown { get; }

        public IAnalyticsEvent<bool> UserFinishedRatingViewFirstStep { get; }

        public IAnalyticsEvent<RatingViewSecondStepOutcome> UserFinishedRatingViewSecondStep { get; }

        public IAnalyticsEvent RatingViewFirstStepLike { get; }

        public IAnalyticsEvent RatingViewFirstStepDislike { get; }

        public IAnalyticsEvent RatingViewSecondStepRate { get; }

        public IAnalyticsEvent RatingViewSecondStepDontRate { get; }

        public IAnalyticsEvent RatingViewSecondStepSendFeedback { get; }

        public IAnalyticsEvent RatingViewSecondStepDontSendFeedback { get; }

        public IAnalyticsEvent<ReportsSource, int, int, double> ReportsSuccess { get; }

        public IAnalyticsEvent<ReportsSource, int, double> ReportsFailure { get; }

        public IAnalyticsEvent OfflineModeDetected { get; }

        public IAnalyticsEvent<EditViewTapSource> EditViewTapped { get; }

        public IAnalyticsEvent<EditViewCloseReason> EditViewClosed { get; }

        public IAnalyticsEvent<int> WorkspacePlaceholdersCreated { get; }

        public IAnalyticsEvent<int> ProjectPlaceholdersCreated { get; }

        public IAnalyticsEvent<int> TaskPlaceholdersCreated { get; }

        public IAnalyticsEvent<int> TagPlaceholdersCreated { get; }

        public IAnalyticsEvent<string, string> HandledException { get; }

        public IAnalyticsEvent TwoRunningTimeEntriesInconsistencyFixed { get; }

        public IAnalyticsEvent<StartViewTapSource> StartViewTapped { get; }

        public IAnalyticsEvent<int> NoDefaultWorkspace { get; }

        public IAnalyticsEvent NoWorkspaces { get; }

        public IAnalyticsEvent<TimeEntryStartOrigin> TimeEntryStarted { get; }

        public IAnalyticsEvent<TimeEntryStopOrigin> TimeEntryStopped { get; }

        public IAnalyticsEvent<ContinueTimeEntryOrigin, int, int, int> TimeEntryContinued { get; }

        public IAnalyticsEvent LostWorkspaceAccess { get; }

        public IAnalyticsEvent GainWorkspaceAccess { get; }

        public IAnalyticsEvent<string> WorkspaceSyncError { get; }

        public IAnalyticsEvent<string> UserSyncError { get; }

        public IAnalyticsEvent<string> WorkspaceFeaturesSyncError { get; }

        public IAnalyticsEvent<string> PreferencesSyncError { get; }

        public IAnalyticsEvent<string> TagsSyncError { get; }

        public IAnalyticsEvent<string> ClientsSyncError { get; }

        public IAnalyticsEvent<string> ProjectsSyncError { get; }

        public IAnalyticsEvent<string> TasksSyncError { get; }

        public IAnalyticsEvent<string> TimeEntrySyncError { get; }

        public IAnalyticsEvent<PushSyncOperation, string> EntitySynced { get; }

        public IAnalyticsEvent<string, string> EntitySyncStatus { get; }

        public IAnalyticsEvent CalendarOnboardingStarted { get; }

        public IAnalyticsEvent<int> NumberOfLinkedCalendarsChanged { get; }

        public IAnalyticsEvent<int> NumberOfLinkedCalendarsNewUser { get; }

        public IAnalyticsEvent EditViewOpenedFromCalendar { get; }

        public IAnalyticsEvent<CalendarChangeEvent> TimeEntryChangedFromCalendar { get; }

        public IAnalyticsEvent<int> ProjectsInaccesibleAfterCleanUp { get; }

        public IAnalyticsEvent<int> TagsInaccesibleAfterCleanUp { get; }

        public IAnalyticsEvent<int> TasksInaccesibleAfterCleanUp { get; }

        public IAnalyticsEvent<int> ClientsInaccesibleAfterCleanUp { get; }

        public IAnalyticsEvent<int> TimeEntriesInaccesibleAfterCleanUp { get; }

        public IAnalyticsEvent<int> WorkspacesInaccesibleAfterCleanUp { get; }

        public IAnalyticsEvent BackgroundSyncStarted { get; }

        public IAnalyticsEvent<string> BackgroundSyncFinished { get; }

        public IAnalyticsEvent<string, string, string> BackgroundSyncFailed { get; }

        public IAnalyticsEvent BackgroundSyncMustStopExcecution { get; }

        public IAnalyticsEvent<string, string> UnknownLoginFailure { get; }

        public IAnalyticsEvent<string, string> UnknownSignUpFailure { get; }

        public IAnalyticsEvent<int> RateLimitingDelayDuringSyncing { get; }

        public IAnalyticsEvent<string> SyncOperationStarted { get; }

        public IAnalyticsEvent SyncCompleted { get; }

        public IAnalyticsEvent LeakyBucketOverflow { get; }

        public IAnalyticsEvent<string, string, string> SyncFailed { get; }

        public IAnalyticsEvent<string> SyncStateTransition { get; }

        public IAnalyticsEvent AppDidEnterForeground { get; }

        public IAnalyticsEvent AppSentToBackground { get; }

        public IAnalyticsEvent<bool> GroupTimeEntriesSettingsChanged { get; }

        public IAnalyticsEvent<EditTimeEntryOrigin> EditViewOpened { get; }

        public IAnalyticsEvent<Platform> ReceivedLowMemoryWarning { get; }

        public IAnalyticsEvent<SuggestionProviderType> SuggestionStarted { get; }

        public IAnalyticsEvent<ApplicationInstallLocation> ApplicationInstallLocation { get; }

        public IAnalyticsEvent<string> PushInitiatedSyncFetch { get; protected set; }

        public IAnalyticsEvent<string> PushNotificationSyncStarted { get; protected set; }

        public IAnalyticsEvent<string> PushNotificationSyncFinished { get; protected set; }

        public IAnalyticsEvent<string, string, string, string> PushNotificationSyncFailed { get; protected set; }

        public IAnalyticsEvent<string, string, string, string> DebugSchedulerError { get; }

        public IAnalyticsEvent<string, string> DebugNavigationError { get; }

        public IAnalyticsEvent<bool> AccessibilityEnabled { get; }

        public IAnalyticsEvent<bool> WatchPaired { get; }

        public IAnalyticsEvent<bool> TimerWidgetInstallStateChange { get; }

        public IAnalyticsEvent<bool> SuggestionsWidgetInstallStateChange { get; }

        public IAnalyticsEvent<int> TimerWidgetSizeChanged { get; }

        public IAnalyticsEvent<CalendarContextualMenuActionType> CalendarEventContextualMenu { get; }

        public IAnalyticsEvent<CalendarContextualMenuActionType> CalendarNewTimeEntryContextualMenu { get; }

        public IAnalyticsEvent<CalendarContextualMenuActionType> CalendarExistingTimeEntryContextualMenu { get; }

        public IAnalyticsEvent<CalendarContextualMenuActionType> CalendarRunningTimeEntryContextualMenu { get; }

        public IAnalyticsEvent<CalendarTimeEntryCreatedType, int, string> CalendarTimeEntryCreated { get; }

        public IAnalyticsEvent<int, string> CalendarWeeklyDatePickerSelectionChanged { get; }

        public IAnalyticsEvent<CalendarSwipeDirection, int, string> CalendarSingleSwipe { get; }

        public IAnalyticsEvent<string> MarketingMessageShown { get; }

        public IAnalyticsEvent<string> MarketingMessageCallToActionHit { get; }

        public IAnalyticsEvent<string> MarketingMessageDismissed { get; }

        protected BaseAnalyticsService()
        {
            Login = new AnalyticsEvent<AuthenticationMethod>(this, nameof(Login), "AuthenticationMethod");
            LoginError = new AnalyticsEvent<LoginErrorSource>(this, nameof(LoginError), "Source");
            SignUp = new AnalyticsEvent<AuthenticationMethod>(this, nameof(SignUp), "AuthenticationMethod");
            SignUpError = new AnalyticsEvent<SignUpErrorSource>(this, nameof(SignUpError), "Source");
            UserIsMissingApiToken = new AnalyticsEvent<LoginSignupAuthenticationMethod>(this, nameof(UserIsMissingApiToken), "AuthenticationMethod");
            OnboardingSkip = new AnalyticsEvent<string>(this, nameof(OnboardingSkip), "PageWhenSkipWasClicked");
            Logout = new AnalyticsEvent<LogoutSource>(this, nameof(Logout), "Source");
            ResetPassword = new AnalyticsEvent(this, nameof(ResetPassword));
            PasswordManagerButtonClicked = new AnalyticsEvent(this, nameof(PasswordManagerButtonClicked));
            PasswordManagerContainsValidEmail = new AnalyticsEvent(this, nameof(PasswordManagerContainsValidEmail));
            PasswordManagerContainsValidPassword = new AnalyticsEvent(this, nameof(PasswordManagerContainsValidPassword));
            CurrentPage = new AnalyticsEvent<Type>(this, nameof(CurrentPage), "CurrentPage");
            DeleteTimeEntry = new AnalyticsEvent<DeleteTimeEntryOrigin>(this, nameof(DeleteTimeEntry), "Source");
            ApplicationShortcut = new AnalyticsEvent<string>(this, nameof(ApplicationShortcut), "ApplicationShortcutType");
            EditEntrySelectProject = new AnalyticsEvent(this, nameof(EditEntrySelectProject));
            EditEntrySelectTag = new AnalyticsEvent(this, nameof(EditEntrySelectTag));
            StartEntrySelectProject = new AnalyticsEvent<ProjectTagSuggestionSource>(this, nameof(StartEntrySelectProject), "Source");
            StartEntrySelectTag = new AnalyticsEvent<ProjectTagSuggestionSource>(this, nameof(StartEntrySelectTag), "Source");
            RatingViewWasShown = new AnalyticsEvent(this, nameof(RatingViewWasShown));
            UserFinishedRatingViewFirstStep = new AnalyticsEvent<bool>(this, nameof(UserFinishedRatingViewFirstStep), "isPositive");
            UserFinishedRatingViewSecondStep = new AnalyticsEvent<RatingViewSecondStepOutcome>(this, nameof(UserFinishedRatingViewSecondStep), "outcome");
            RatingViewFirstStepLike = new AnalyticsEvent(this, nameof(RatingViewFirstStepLike));
            RatingViewFirstStepDislike = new AnalyticsEvent(this, nameof(RatingViewFirstStepDislike));
            RatingViewSecondStepRate = new AnalyticsEvent(this, nameof(RatingViewSecondStepRate));
            RatingViewSecondStepDontRate = new AnalyticsEvent(this, nameof(RatingViewSecondStepDontRate));
            RatingViewSecondStepSendFeedback = new AnalyticsEvent(this, nameof(RatingViewSecondStepSendFeedback));
            RatingViewSecondStepDontSendFeedback = new AnalyticsEvent(this, nameof(RatingViewSecondStepDontSendFeedback));
            ReportsSuccess = new AnalyticsEvent<ReportsSource, int, int, double>(this, nameof(ReportsSuccess), "Source", "TotalDays", "ProjectsNotSynced", "LoadingTime");
            ReportsFailure = new AnalyticsEvent<ReportsSource, int, double>(this, nameof(ReportsFailure), "Source", "TotalDays", "LoadingTime");
            OfflineModeDetected = new AnalyticsEvent(this, nameof(OfflineModeDetected));
            EditViewTapped = new AnalyticsEvent<EditViewTapSource>(this, nameof(EditViewTapped), "TapSource");
            EditViewClosed = new AnalyticsEvent<EditViewCloseReason>(this, nameof(EditViewClosed), "Reason");
            WorkspacePlaceholdersCreated = new AnalyticsEvent<int>(this, nameof(WorkspacePlaceholdersCreated), "NumberOfCreatedPlaceholders");
            ProjectPlaceholdersCreated = new AnalyticsEvent<int>(this, nameof(ProjectPlaceholdersCreated), "NumberOfCreatedPlaceholders");
            TaskPlaceholdersCreated = new AnalyticsEvent<int>(this, nameof(TaskPlaceholdersCreated), "NumberOfCreatedPlaceholders");
            TagPlaceholdersCreated = new AnalyticsEvent<int>(this, nameof(TagPlaceholdersCreated), "NumberOfCreatedPlaceholders");
            HandledException = new AnalyticsEvent<string, string>(this, nameof(HandledException), "ExceptionType", "ExceptionMessage");
            TwoRunningTimeEntriesInconsistencyFixed = new AnalyticsEvent(this, nameof(TwoRunningTimeEntriesInconsistencyFixed));
            StartViewTapped = new AnalyticsEvent<StartViewTapSource>(this, nameof(StartViewTapped), "TapSource");
            NoDefaultWorkspace = new AnalyticsEvent<int>(this, nameof(NoDefaultWorkspace), "NumberOfWorkspaces");
            NoWorkspaces = new AnalyticsEvent(this, nameof(NoWorkspaces));
            TimeEntryStarted = new AnalyticsEvent<TimeEntryStartOrigin>(this, nameof(TimeEntryStarted), "Origin");
            TimeEntryStopped = new AnalyticsEvent<TimeEntryStopOrigin>(this, nameof(TimeEntryStopped), "Origin");
            TimeEntryContinued = new AnalyticsEvent<ContinueTimeEntryOrigin, int, int, int>(this, nameof(TimeEntryContinued), "Origin", "IndexInLog", "DayInLog", "DaysInThePast");
            LostWorkspaceAccess = new AnalyticsEvent(this, nameof(LostWorkspaceAccess));
            GainWorkspaceAccess = new AnalyticsEvent(this, nameof(GainWorkspaceAccess));
            WorkspaceSyncError = new AnalyticsEvent<string>(this, nameof(WorkspaceSyncError), "Reason");
            UserSyncError = new AnalyticsEvent<string>(this, nameof(UserSyncError), "Reason");
            WorkspaceFeaturesSyncError = new AnalyticsEvent<string>(this, nameof(WorkspaceFeaturesSyncError), "Reason");
            PreferencesSyncError = new AnalyticsEvent<string>(this, nameof(PreferencesSyncError), "Reason");
            TagsSyncError = new AnalyticsEvent<string>(this, nameof(TagsSyncError), "Reason");
            ClientsSyncError = new AnalyticsEvent<string>(this, nameof(ClientsSyncError), "Reason");
            ProjectsSyncError = new AnalyticsEvent<string>(this, nameof(ProjectsSyncError), "Reason");
            TasksSyncError = new AnalyticsEvent<string>(this, nameof(TasksSyncError), "Reason");
            TimeEntrySyncError = new AnalyticsEvent<string>(this, nameof(TimeEntrySyncError), "Reason");
            EntitySynced = new AnalyticsEvent<PushSyncOperation, string>(this, nameof(EntitySynced), "Method", "Entity");
            EntitySyncStatus = new AnalyticsEvent<string, string>(this, nameof(EntitySyncStatus), "Entity", "Status");
            CalendarOnboardingStarted = new AnalyticsEvent(this, nameof(CalendarOnboardingStarted));
            NumberOfLinkedCalendarsChanged = new AnalyticsEvent<int>(this, nameof(NumberOfLinkedCalendarsChanged), "Count");
            NumberOfLinkedCalendarsNewUser = new AnalyticsEvent<int>(this, nameof(NumberOfLinkedCalendarsNewUser), "Count");
            EditViewOpenedFromCalendar = new AnalyticsEvent(this, nameof(EditViewOpenedFromCalendar));
            TimeEntryChangedFromCalendar = new AnalyticsEvent<CalendarChangeEvent>(this, nameof(TimeEntryChangedFromCalendar), "ChangeEvent");
            ProjectsInaccesibleAfterCleanUp = new AnalyticsEvent<int>(this, nameof(ProjectsInaccesibleAfterCleanUp), "NumberOfProjectsInaccesibleAfterCleanUp");
            TagsInaccesibleAfterCleanUp = new AnalyticsEvent<int>(this, nameof(TagsInaccesibleAfterCleanUp), "NumberOfTagsInaccesibleAfterCleanUp");
            TasksInaccesibleAfterCleanUp = new AnalyticsEvent<int>(this, nameof(TasksInaccesibleAfterCleanUp), "NumberOfTasksInaccesibleAfterCleanUp");
            ClientsInaccesibleAfterCleanUp = new AnalyticsEvent<int>(this, nameof(ClientsInaccesibleAfterCleanUp), "NumberOfClientsInaccesibleAfterCleanUp");
            TimeEntriesInaccesibleAfterCleanUp = new AnalyticsEvent<int>(this, nameof(TimeEntriesInaccesibleAfterCleanUp), "NumberOfTimeEntriesInaccesibleAfterCleanUp");
            WorkspacesInaccesibleAfterCleanUp = new AnalyticsEvent<int>(this, nameof(WorkspacesInaccesibleAfterCleanUp), "NumberOfWorkspacesInaccesibleAfterCleanUp");
            BackgroundSyncStarted = new AnalyticsEvent(this, nameof(BackgroundSyncStarted));
            BackgroundSyncFinished = new AnalyticsEvent<string>(this, nameof(BackgroundSyncFinished), "BackgroundSyncFinishedWithOutcome");
            BackgroundSyncFailed = new AnalyticsEvent<string, string, string>(this, nameof(BackgroundSyncFailed), "Type", "Message", "StackTrace");
            BackgroundSyncMustStopExcecution = new AnalyticsEvent(this, nameof(BackgroundSyncMustStopExcecution));
            UnknownLoginFailure = new AnalyticsEvent<string, string>(this, nameof(UnknownLoginFailure), "Type", "Message");
            UnknownSignUpFailure = new AnalyticsEvent<string, string>(this, nameof(UnknownSignUpFailure), "Type", "Message");
            RateLimitingDelayDuringSyncing = new AnalyticsEvent<int>(this, nameof(RateLimitingDelayDuringSyncing), "DelayDurationSeconds");
            SyncOperationStarted = new AnalyticsEvent<string>(this, nameof(SyncOperationStarted), "State");
            SyncCompleted = new AnalyticsEvent(this, nameof(SyncCompleted));
            LeakyBucketOverflow = new AnalyticsEvent(this, nameof(LeakyBucketOverflow));
            SyncFailed = new AnalyticsEvent<string, string, string>(this, nameof(SyncFailed), "Type", "Message", "StackTrace");
            SyncStateTransition = new AnalyticsEvent<string>(this, nameof(SyncStateTransition), "StateName");
            AppDidEnterForeground = new AnalyticsEvent(this, nameof(AppDidEnterForeground));
            AppSentToBackground = new AnalyticsEvent(this, nameof(AppSentToBackground));
            GroupTimeEntriesSettingsChanged = new AnalyticsEvent<bool>(this, nameof(GroupTimeEntriesSettingsChanged), "State");
            EditViewOpened = new AnalyticsEvent<EditTimeEntryOrigin>(this, nameof(EditViewOpened), "Origin");
            ReceivedLowMemoryWarning = new AnalyticsEvent<Platform>(this, nameof(ReceivedLowMemoryWarning), "Platform");
            SuggestionStarted = new AnalyticsEvent<SuggestionProviderType>(this, nameof(SuggestionStarted), "SuggestionProvider");
            ApplicationInstallLocation = new AnalyticsEvent<ApplicationInstallLocation>(this, nameof(ApplicationInstallLocation), "Location");
            DebugSchedulerError = new AnalyticsEvent<string, string, string, string>(this, nameof(DebugSchedulerError), "Type", "Source", "ExceptionType", "StackTrace");
            DebugNavigationError = new AnalyticsEvent<string, string>(this, nameof(DebugNavigationError), "Action", "Type");
            AccessibilityEnabled = new AnalyticsEvent<bool>(this, nameof(AccessibilityEnabled), "Enabled");
            WatchPaired = new AnalyticsEvent<bool>(this, nameof(WatchPaired), "Installed");
            TimerWidgetInstallStateChange = new AnalyticsEvent<bool>(this, nameof(TimerWidgetInstallStateChange), "Installed");
            SuggestionsWidgetInstallStateChange = new AnalyticsEvent<bool>(this, nameof(SuggestionsWidgetInstallStateChange), "Installed");
            TimerWidgetSizeChanged = new AnalyticsEvent<int>(this, nameof(TimerWidgetSizeChanged), "Columns");
            PushInitiatedSyncFetch = new AnalyticsEvent<string>(this, nameof(PushInitiatedSyncFetch), "NumberOfEntitiesFetched");
            PushNotificationSyncStarted = new AnalyticsEvent<string>(this, nameof(PushNotificationSyncStarted), "Source");
            PushNotificationSyncFinished = new AnalyticsEvent<string>(this, nameof(PushNotificationSyncFinished), "Source");
            CalendarWeeklyDatePickerSelectionChanged = new AnalyticsEvent<int, string>(this, nameof(CalendarWeeklyDatePickerSelectionChanged), "DaysSinceToday", "DayOfWeek");
            CalendarSingleSwipe = new AnalyticsEvent<CalendarSwipeDirection, int, string>(this, nameof(CalendarSingleSwipe), "SwipeDirection", "DaysSinceToday", "DayOfWeek");
            PushNotificationSyncFailed = new AnalyticsEvent<string, string, string, string>(this, nameof(PushNotificationSyncFailed), "Source", "Type", "Message", "StackTrace");
            CalendarEventContextualMenu = new AnalyticsEvent<CalendarContextualMenuActionType>(this, nameof(CalendarEventContextualMenu), "SelectedOption");
            CalendarNewTimeEntryContextualMenu = new AnalyticsEvent<CalendarContextualMenuActionType>(this, nameof(CalendarNewTimeEntryContextualMenu), "SelectedOption");
            CalendarExistingTimeEntryContextualMenu = new AnalyticsEvent<CalendarContextualMenuActionType>(this, nameof(CalendarExistingTimeEntryContextualMenu), "SelectedOption");
            CalendarRunningTimeEntryContextualMenu = new AnalyticsEvent<CalendarContextualMenuActionType>(this, nameof(CalendarRunningTimeEntryContextualMenu), "SelectedOption");
            CalendarTimeEntryCreated = new AnalyticsEvent<CalendarTimeEntryCreatedType, int, string>(this, nameof(CalendarTimeEntryCreated), "Type", "DaysSinceToday", "DayOfTheWeek");
            MarketingMessageShown = new AnalyticsEvent<string>(this, nameof(MarketingMessageShown), "Campaign");
            MarketingMessageCallToActionHit = new AnalyticsEvent<string>(this, nameof(MarketingMessageCallToActionHit), "Campaign");
            MarketingMessageDismissed = new AnalyticsEvent<string>(this, nameof(MarketingMessageDismissed), "Campaign");
        }

        public void TrackAnonymized(Exception exception)
        {
            if (exception.IsAnonymized())
            {
                TrackException(exception);
            }
            else
            {
                HandledException.Track(exception.GetType().FullName, exception.Message);
            }
        }

        public abstract void Track(Exception exception, string message);

        public abstract void Track(Exception exception, IDictionary<string, string> properties);

        public abstract void Track(string eventName, Dictionary<string, string> parameters = null);

        public void Track(ITrackableEvent trackableEvent)
            => Track(trackableEvent.EventName, trackableEvent.ToDictionary());

        protected abstract void TrackException(Exception exception);

        public abstract void SetAppCenterUserId(long id);
        public abstract void ResetAppCenterUserId();
    }
}
