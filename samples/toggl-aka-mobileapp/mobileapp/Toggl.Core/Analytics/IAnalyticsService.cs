using System;
using System.Collections.Generic;
using Toggl.Core.Suggestions;
using Toggl.Core.Sync;

namespace Toggl.Core.Analytics
{
    public interface IAnalyticsService
    {
        IAnalyticsEvent<AuthenticationMethod> Login { get; }

        IAnalyticsEvent<LoginErrorSource> LoginError { get; }

        IAnalyticsEvent<AuthenticationMethod> SignUp { get; }

        IAnalyticsEvent<SignUpErrorSource> SignUpError { get; }

        IAnalyticsEvent<LoginSignupAuthenticationMethod> UserIsMissingApiToken { get; }

        IAnalyticsEvent<string> OnboardingSkip { get; }

        IAnalyticsEvent<LogoutSource> Logout { get; }

        IAnalyticsEvent ResetPassword { get; }

        IAnalyticsEvent PasswordManagerButtonClicked { get; }

        IAnalyticsEvent PasswordManagerContainsValidEmail { get; }

        IAnalyticsEvent PasswordManagerContainsValidPassword { get; }

        IAnalyticsEvent<Type> CurrentPage { get; }

        IAnalyticsEvent<TimeEntryStartOrigin> TimeEntryStarted { get; }

        IAnalyticsEvent<TimeEntryStopOrigin> TimeEntryStopped { get; }

        IAnalyticsEvent<ContinueTimeEntryOrigin, int, int, int> TimeEntryContinued { get; }

        IAnalyticsEvent RatingViewWasShown { get; }

        IAnalyticsEvent<bool> UserFinishedRatingViewFirstStep { get; }

        IAnalyticsEvent<RatingViewSecondStepOutcome> UserFinishedRatingViewSecondStep { get; }

        IAnalyticsEvent RatingViewFirstStepLike { get; }

        IAnalyticsEvent RatingViewFirstStepDislike { get; }

        IAnalyticsEvent RatingViewSecondStepRate { get; }

        IAnalyticsEvent RatingViewSecondStepDontRate { get; }

        IAnalyticsEvent RatingViewSecondStepSendFeedback { get; }

        IAnalyticsEvent RatingViewSecondStepDontSendFeedback { get; }

        IAnalyticsEvent<DeleteTimeEntryOrigin> DeleteTimeEntry { get; }

        IAnalyticsEvent<string> ApplicationShortcut { get; }

        IAnalyticsEvent EditEntrySelectProject { get; }

        IAnalyticsEvent EditEntrySelectTag { get; }

        IAnalyticsEvent<ProjectTagSuggestionSource> StartEntrySelectProject { get; }

        IAnalyticsEvent<ProjectTagSuggestionSource> StartEntrySelectTag { get; }

        IAnalyticsEvent<ReportsSource, int, int, double> ReportsSuccess { get; }

        IAnalyticsEvent<ReportsSource, int, double> ReportsFailure { get; }

        IAnalyticsEvent OfflineModeDetected { get; }

        IAnalyticsEvent<int> WorkspacePlaceholdersCreated { get; }

        IAnalyticsEvent<int> ProjectPlaceholdersCreated { get; }

        IAnalyticsEvent<int> TaskPlaceholdersCreated { get; }

        IAnalyticsEvent<int> TagPlaceholdersCreated { get; }

        IAnalyticsEvent<EditViewTapSource> EditViewTapped { get; }

        IAnalyticsEvent<EditViewCloseReason> EditViewClosed { get; }

        IAnalyticsEvent<StartViewTapSource> StartViewTapped { get; }

        IAnalyticsEvent LostWorkspaceAccess { get; }

        IAnalyticsEvent GainWorkspaceAccess { get; }

        IAnalyticsEvent<string> WorkspaceSyncError { get; }

        IAnalyticsEvent<string> UserSyncError { get; }

        IAnalyticsEvent<string> WorkspaceFeaturesSyncError { get; }

        IAnalyticsEvent<string> PreferencesSyncError { get; }

        IAnalyticsEvent<string> TagsSyncError { get; }

        IAnalyticsEvent<string> ClientsSyncError { get; }

        IAnalyticsEvent<string> ProjectsSyncError { get; }

        IAnalyticsEvent<string> TasksSyncError { get; }

        IAnalyticsEvent<string> TimeEntrySyncError { get; }

        IAnalyticsEvent<PushSyncOperation, string> EntitySynced { get; }

        IAnalyticsEvent<string, string> EntitySyncStatus { get; }

        IAnalyticsEvent<int> NoDefaultWorkspace { get; }

        IAnalyticsEvent NoWorkspaces { get; }

        IAnalyticsEvent<string, string> HandledException { get; }

        IAnalyticsEvent TwoRunningTimeEntriesInconsistencyFixed { get; }

        IAnalyticsEvent CalendarOnboardingStarted { get; }

        IAnalyticsEvent<int> NumberOfLinkedCalendarsChanged { get; }

        IAnalyticsEvent<int> NumberOfLinkedCalendarsNewUser { get; }

        IAnalyticsEvent EditViewOpenedFromCalendar { get; }

        IAnalyticsEvent<CalendarChangeEvent> TimeEntryChangedFromCalendar { get; }

        IAnalyticsEvent<int> ProjectsInaccesibleAfterCleanUp { get; }

        IAnalyticsEvent<int> TagsInaccesibleAfterCleanUp { get; }

        IAnalyticsEvent<int> TasksInaccesibleAfterCleanUp { get; }

        IAnalyticsEvent<int> ClientsInaccesibleAfterCleanUp { get; }

        IAnalyticsEvent<int> TimeEntriesInaccesibleAfterCleanUp { get; }

        IAnalyticsEvent<int> WorkspacesInaccesibleAfterCleanUp { get; }

        IAnalyticsEvent BackgroundSyncStarted { get; }

        IAnalyticsEvent<string> BackgroundSyncFinished { get; }

        IAnalyticsEvent<string, string, string> BackgroundSyncFailed { get; }

        IAnalyticsEvent BackgroundSyncMustStopExcecution { get; }

        IAnalyticsEvent<int> RateLimitingDelayDuringSyncing { get; }

        IAnalyticsEvent<string, string> UnknownLoginFailure { get; }

        IAnalyticsEvent<string, string> UnknownSignUpFailure { get; }

        IAnalyticsEvent<string> SyncOperationStarted { get; }

        IAnalyticsEvent SyncCompleted { get; }

        IAnalyticsEvent LeakyBucketOverflow { get; }

        IAnalyticsEvent<string, string, string> SyncFailed { get; }

        IAnalyticsEvent<string> SyncStateTransition { get; }

        IAnalyticsEvent AppDidEnterForeground { get; }

        IAnalyticsEvent AppSentToBackground { get; }

        IAnalyticsEvent<bool> GroupTimeEntriesSettingsChanged { get; }

        IAnalyticsEvent<EditTimeEntryOrigin> EditViewOpened { get; }

        IAnalyticsEvent<Platform> ReceivedLowMemoryWarning { get; }

        IAnalyticsEvent<string> PushInitiatedSyncFetch { get; }

        IAnalyticsEvent<string> PushNotificationSyncStarted { get; }

        IAnalyticsEvent<string> PushNotificationSyncFinished { get; }

        IAnalyticsEvent<string, string, string, string> PushNotificationSyncFailed { get; }

        IAnalyticsEvent<SuggestionProviderType> SuggestionStarted { get; }

        IAnalyticsEvent<ApplicationInstallLocation> ApplicationInstallLocation { get; }

        IAnalyticsEvent<string, string, string, string> DebugSchedulerError { get; }

        IAnalyticsEvent<string, string> DebugNavigationError { get; }

        IAnalyticsEvent<bool> AccessibilityEnabled { get; }

        IAnalyticsEvent<bool> WatchPaired { get; }

        IAnalyticsEvent<bool> TimerWidgetInstallStateChange { get; }

        IAnalyticsEvent<bool> SuggestionsWidgetInstallStateChange { get; }

        IAnalyticsEvent<int> TimerWidgetSizeChanged { get; }

        IAnalyticsEvent<CalendarContextualMenuActionType> CalendarEventContextualMenu { get; }

        IAnalyticsEvent<CalendarContextualMenuActionType> CalendarNewTimeEntryContextualMenu { get; }

        IAnalyticsEvent<CalendarContextualMenuActionType> CalendarExistingTimeEntryContextualMenu { get; }

        IAnalyticsEvent<CalendarContextualMenuActionType> CalendarRunningTimeEntryContextualMenu { get; }

        IAnalyticsEvent<CalendarTimeEntryCreatedType, int, string> CalendarTimeEntryCreated { get; }

        IAnalyticsEvent<int, string> CalendarWeeklyDatePickerSelectionChanged { get; }

        IAnalyticsEvent<CalendarSwipeDirection, int, string> CalendarSingleSwipe { get; }

        IAnalyticsEvent<string> MarketingMessageShown { get; }

        IAnalyticsEvent<string> MarketingMessageCallToActionHit { get; }

        IAnalyticsEvent<string> MarketingMessageDismissed { get; }

        void SetAppCenterUserId(long id);
        void ResetAppCenterUserId();

        void Track(string eventName, Dictionary<string, string> parameters = null);

        void TrackAnonymized(Exception exception);

        void Track(Exception exception, string message);

        void Track(Exception exception, IDictionary<string, string> properties);

        void Track(ITrackableEvent trackableEvent);
    }
}
