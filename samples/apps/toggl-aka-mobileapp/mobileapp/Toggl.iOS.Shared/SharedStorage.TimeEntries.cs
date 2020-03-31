using Foundation;
using Toggl.iOS.Shared.Models;
using Toggl.iOS.Shared.Extensions;
using Toggl.Shared.Models;
using System;

namespace Toggl.iOS.Shared
{
    public partial class SharedStorage
    {
        private const string timeEntryId = "Id";
        private const string timeEntryWorkspaceId = "WorkspaceId";
        private const string timeEntryUserId = "UserId";
        private const string timeEntryProjectId = "ProjectId";
        private const string timeEntryProjectName = "ProjectName";
        private const string timeEntryProjectColor = "ProjectColor";
        private const string timeEntryTaskId = "TaskId";
        private const string timeEntryTaskName = "TaskName";
        private const string timeEntryIsBillable = "IsBillable";
        private const string timeEntryDescription = "Description";
        private const string timeEntryClientName = "ClientName";
        private const string timeEntryStartTime = "StartTime";
        private const string timeEntryDuration = "Duration";
        private const string timeEntryServerDeletedAt = "ServerDeletedAt";
        private const string timeEntryAt = "At";

        private IDisposable currentTimeEntryObservingDisposable;

        public void SetRunningTimeEntry(ITimeEntry timeEntry, string projectName = "", string projectColor = "", string taskName = "", string clientName = "")
        {
            if (timeEntry == null)
            {
                userDefaults.RemoveObject(runningTimeEntry);
                return;
            }

            var dict = new NSMutableDictionary();
            dict.SetLongForKey(timeEntry.Id, timeEntryId);
            dict.SetLongForKey(timeEntry.WorkspaceId, timeEntryWorkspaceId);
            dict.SetLongForKey(timeEntry.UserId, timeEntryUserId);

            dict.SetStringForKey(timeEntry.Description, timeEntryDescription);
            dict.SetDateTimeOffsetForKey(timeEntry.Start, timeEntryStartTime);

            if (timeEntry.Duration.HasValue)
                dict.SetLongForKey(timeEntry.Duration.Value, timeEntryDuration);

            dict.SetBoolForKey(timeEntry.Billable, timeEntryIsBillable);

            if (timeEntry.ProjectId.HasValue)
            {
                dict.SetLongForKey(timeEntry.ProjectId.Value, timeEntryProjectId);
                dict.SetStringForKey(projectName, timeEntryProjectName);
                dict.SetStringForKey(projectColor, timeEntryProjectColor);
                dict.SetStringForKey(clientName, timeEntryClientName);
            }

            if (timeEntry.TaskId.HasValue)
            {
                dict.SetLongForKey(timeEntry.TaskId.Value, timeEntryTaskId);
                dict.SetStringForKey(taskName, timeEntryTaskName);
            }

            if (timeEntry.ServerDeletedAt.HasValue)
                dict.SetDateTimeOffsetForKey(timeEntry.ServerDeletedAt.Value, timeEntryServerDeletedAt);

            dict.SetDateTimeOffsetForKey(timeEntry.At, timeEntryAt);

            userDefaults[new NSString(runningTimeEntry)] = dict;
            userDefaults.Synchronize();
        }

        public TimeEntryViewModel GetRunningTimeEntryViewModel()
        {
            var dict = userDefaults.ValueForKey(new NSString(runningTimeEntry)) as NSDictionary;
            return getTimeEntryViewModel(dict);
        }

        public void ObserveChangesToCurrentRunningTimeEntry(Action<TimeEntryViewModel> onUpdate)
        {
            currentTimeEntryObservingDisposable?.Dispose();
            currentTimeEntryObservingDisposable =
                userDefaults.AddObserver(
                    runningTimeEntry,
                    NSKeyValueObservingOptions.New,
                    change =>
                    {
                        var dict = change.NewValue as NSDictionary;
                        onUpdate(getTimeEntryViewModel(dict));
                    });
        }

        public void StopObservingChangesToCurrentRunningTimeEntry()
        {
            currentTimeEntryObservingDisposable?.Dispose();
            currentTimeEntryObservingDisposable = null;
        }

        private ITimeEntry getTimeEntry(NSDictionary dict)
        {
            if (dict == null)
                return null;

            var id = dict.GetLongForKey(timeEntryId).Value;
            var workspaceId = dict.GetLongForKey(timeEntryWorkspaceId).Value;
            var userId = dict.GetLongForKey(timeEntryUserId).Value;

            var description = dict.GetStringForKey(timeEntryDescription);
            var startTime = dict.GetDateTimeOffsetForKey(timeEntryStartTime).Value;
            var duration = dict.GetLongForKey(timeEntryDuration);
            var isBillable = dict.GetBoolForKey(timeEntryIsBillable).Value;

            var projectId = dict.GetLongForKey(timeEntryProjectId);
            var taskId = dict.GetLongForKey(timeEntryTaskId);
            var serverDeletedAt = dict.GetDateTimeOffsetForKey(timeEntryServerDeletedAt);
            var at = dict.GetDateTimeOffsetForKey(timeEntryAt).Value;

            return new TimeEntry(
                workspaceId,
                projectId,
                taskId,
                isBillable,
                startTime,
                duration,
                description,
                null,
                userId,
                id,
                serverDeletedAt,
                at);
        }

        private TimeEntryViewModel getTimeEntryViewModel(NSDictionary dict)
        {
            if (dict == null)
                return null;

            var timeEntry = getTimeEntry(dict);

            if (timeEntry == null)
                return null;

            var projectName = dict.GetStringForKey(timeEntryProjectName, "");
            var projectColor = dict.GetStringForKey(timeEntryProjectColor, "");
            var taskName = dict.GetStringForKey(timeEntryTaskName, "");
            var clientName = dict.GetStringForKey(timeEntryClientName,  "");

            return new TimeEntryViewModel(timeEntry, projectName, projectColor, taskName, clientName);
        }
    }
}
