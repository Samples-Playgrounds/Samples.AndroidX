using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Autocomplete;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Core.Calendar;
using Toggl.Core.DTOs;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Reports;
using Toggl.Core.Search;
using Toggl.Core.Suggestions;
using Toggl.Shared;
using Toggl.Shared.Models.Reports;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Core.Interactors
{
    public interface IInteractorFactory
    {
        #region Time Entries

        IInteractor<Task<IThreadSafeTimeEntry>> CreateTimeEntry(ITimeEntryPrototype prototype, TimeEntryStartOrigin origin);

        IInteractor<Task<IThreadSafeTimeEntry>> StartSuggestion(Suggestion suggestion);

        IInteractor<Task<IThreadSafeTimeEntry>> ContinueTimeEntry(long timeEntryId, ContinueTimeEntryMode continueMode);

        IInteractor<Task<IThreadSafeTimeEntry>> ContinueMostRecentTimeEntry();

        IInteractor<Task<IThreadSafeTimeEntry>> UpdateTimeEntry(EditTimeEntryDto dto);

        IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>> UpdateMultipleTimeEntries(EditTimeEntryDto[] dtos);

        IInteractor<IObservable<IThreadSafeTimeEntry>> GetTimeEntryById(long id);

        IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>> GetMultipleTimeEntriesById(long[] ids);

        IInteractor<Task> DeleteTimeEntry(long id);

        IInteractor<Task> DeleteMultipleTimeEntries(long[] ids);

        IInteractor<IObservable<Unit>> SoftDeleteMultipleTimeEntries(long[] ids);

        IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>> GetAllTimeEntriesVisibleToTheUser();

        IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>> ObserveAllTimeEntriesVisibleToTheUser();

        IInteractor<Task<IThreadSafeTimeEntry>> StopTimeEntry(DateTimeOffset currentDateTime, TimeEntryStopOrigin origin);

        IInteractor<IObservable<Unit>> ObserveTimeEntriesChanges();

        IInteractor<IObservable<TimeSpan>> ObserveTimeTrackedToday();

        #endregion

        #region Projects

        IInteractor<IObservable<bool>> ProjectDefaultsToBillable(long projectId);

        IInteractor<IObservable<bool>> IsBillableAvailableForProject(long projectId);

        IInteractor<IObservable<IThreadSafeProject>> CreateProject(CreateProjectDTO dto);

        IInteractor<IObservable<IThreadSafeProject>> GetProjectById(long id);

        #endregion

        #region Workspaces

        IInteractor<IObservable<IThreadSafeWorkspace>> GetDefaultWorkspace();

        IInteractor<IObservable<Unit>> SetDefaultWorkspace(long workspaceId);

        IInteractor<IObservable<IEnumerable<IThreadSafeWorkspace>>> GetAllWorkspaces();

        IInteractor<IObservable<IThreadSafeWorkspace>> GetWorkspaceById(long workspaceId);

        IInteractor<IObservable<bool?>> AreProjectsBillableByDefault(long workspaceId);

        IInteractor<IObservable<bool>> WorkspaceAllowsBillableRates(long workspaceId);

        IInteractor<IObservable<bool>> AreCustomColorsEnabledForWorkspace(long workspaceId);

        IInteractor<IObservable<bool>> IsBillableAvailableForWorkspace(long workspaceId);

        IInteractor<IObservable<Unit>> CreateDefaultWorkspace();

        IInteractor<IObservable<IEnumerable<IThreadSafeWorkspace>>> ObserveAllWorkspaces();

        IInteractor<IObservable<Unit>> ObserveWorkspacesChanges();

        IInteractor<IObservable<long>> ObserveDefaultWorkspaceId();

        #endregion

        #region WorkspaceFeatureCollection

        IInteractor<IObservable<IThreadSafeWorkspaceFeatureCollection>> GetWorkspaceFeaturesById(long id);

        #endregion

        #region Sync

        IInteractor<IObservable<IEnumerable<SyncFailureItem>>> GetItemsThatFailedToSync();

        IInteractor<IObservable<bool>> HasFinishedSyncBefore();

        IInteractor<IObservable<SyncOutcome>> RunBackgroundSync();

        IInteractor<IObservable<bool>> ContainsPlaceholders();

        IInteractor<IObservable<SyncOutcome>> RunPushNotificationInitiatedSyncInForeground();

        IInteractor<IObservable<SyncOutcome>> RunPushNotificationInitiatedSyncInBackground();

        #endregion

        #region Autocomplete Suggestions

        IInteractor<IObservable<IEnumerable<AutocompleteSuggestion>>> GetAutocompleteSuggestions(
            QueryInfo queryInfo, ISearchEngine<IThreadSafeTimeEntry> searchEngine);

        IInteractor<IObservable<IEnumerable<AutocompleteSuggestion>>> GetTagsAutocompleteSuggestions(
            IList<string> wordsToQuery);

        IInteractor<IObservable<IEnumerable<AutocompleteSuggestion>>> GetProjectsAutocompleteSuggestions(
            IList<string> wordsToQuery);

        #endregion

        #region Preferences

        IInteractor<IObservable<IThreadSafePreferences>> GetPreferences();

        IInteractor<IObservable<IThreadSafePreferences>> UpdatePreferences(EditPreferencesDTO dto);

        #endregion

        #region User

        IInteractor<IObservable<IThreadSafeUser>> GetCurrentUser();

        IInteractor<IObservable<IThreadSafeUser>> UpdateUser(EditUserDTO dto);

        IInteractor<IObservable<IThreadSafeUser>> UpdateDefaultWorkspace(long selectedWorkspaceId);

        #endregion

        #region UserAccess

        IInteractor<IObservable<Unit>> Logout(LogoutSource source);

        #endregion

        #region Settings

        IInteractor<IObservable<Unit>> SendFeedback(string message);

        #endregion

        #region Calendar

        IInteractor<IObservable<CalendarItem>> GetCalendarItemWithId(string eventId);

        IInteractor<IObservable<IEnumerable<CalendarItem>>> GetCalendarItemsForDate(DateTime date);

        IInteractor<IObservable<IEnumerable<UserCalendar>>> GetUserCalendars();

        IInteractor<Unit> SetEnabledCalendars(params string[] ids);

        #endregion

        #region Notifications

        IInteractor<IObservable<Unit>> UnscheduleAllNotifications();

        IInteractor<IObservable<Unit>> ScheduleEventNotificationsForNextWeek();

        #endregion

        #region Clients

        IInteractor<IObservable<IThreadSafeClient>> CreateClient(string clientName, long workspaceId);

        IInteractor<IObservable<IEnumerable<IThreadSafeClient>>> GetAllClientsInWorkspace(long workspaceId);

        IInteractor<IObservable<IThreadSafeClient>> GetClientById(long id);

        #endregion

        #region Tags

        IInteractor<IObservable<IThreadSafeTag>> CreateTag(string tagName, long workspaceId);

        IInteractor<IObservable<IThreadSafeTag>> GetTagById(long id);

        IInteractor<IObservable<IEnumerable<IThreadSafeTag>>> GetMultipleTagsById(params long[] id);

        #endregion

        #region Tasks

        IInteractor<IObservable<IThreadSafeTask>> GetTaskById(long id);

        #endregion

        #region Changes

        IInteractor<IObservable<Unit>> ObserveWorkspaceOrTimeEntriesChanges();

        #endregion

        #region Timezones

        IInteractor<IObservable<IEnumerable<string>>> GetSupportedTimezones();

        #endregion

        #region Reports

        IInteractor<IObservable<ITimeEntriesTotals>> GetReportsTotals(
            long userId, long workspaceId, DateTimeOffset startDate, DateTimeOffset endDate);

        IInteractor<IObservable<ProjectSummaryReport>> GetProjectSummary(
            long workspaceId, DateTimeOffset startDate, DateTimeOffset? endDate);

        #endregion

        #region Suggestions

        IInteractor<IObservable<IEnumerable<Suggestion>>> GetSuggestions(int count);
        IInteractor<IObservable<IReadOnlyList<ISuggestionProvider>>> GetSuggestionProviders(int count);

        #endregion

        #region PushNotifications

        IInteractor<IObservable<Unit>> UnsubscribeFromPushNotifications();

        IInteractor<IObservable<Unit>> SubscribeToPushNotifications();

        #endregion
    }
}
