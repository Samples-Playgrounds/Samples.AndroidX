using System;
using Toggl.Core.Analytics;

namespace Toggl.Core.UI.Parameters
{
    public class DeeplinkParameters
    {
        public static DeeplinkParameters Other => new DeeplinkParameters();

        public static DeeplinkParameters WithStartTimeEntry(
            string description,
            DateTimeOffset? startTime,
            long? workspaceId,
            long? projectId,
            long? taskId,
            long[] tagIds,
            bool? isBillable,
            TimeEntryStartOrigin? source)
            => new DeeplinkStartTimeEntryParameters(description, startTime, workspaceId, projectId, taskId, tagIds, isBillable, source);

        public static DeeplinkParameters WithContinueLast()
            => new DeeplinkContinueTimeEntryParameters();

        public static DeeplinkParameters WithStopTimeEntry(
            DateTimeOffset? stopTime = null,
            TimeEntryStopOrigin? source = null)
            => new DeeplinkStopTimeEntryParameters(stopTime, source);

        public static DeeplinkParameters WithCreateTimeEntry(
            string description = "",
            DateTimeOffset? startTime = null,
            DateTimeOffset? stopTime = null,
            TimeSpan? duration = null,
            long? workspaceId = null,
            long? projectId = null,
            long? taskId = null,
            long[] tagIds = null,
            bool? isBillable = null,
            TimeEntryStartOrigin? source = null)
            => new DeeplinkCreateTimeEntryParameters(description, startTime, stopTime, duration, workspaceId, projectId, taskId, tagIds, isBillable, source);

        public static DeeplinkParameters WithUpdateTimeEntry(
            long timeEntryId,
            bool hasDescription, string description,
            bool hasStartTime, DateTimeOffset? startTime,
            bool hasStopTime, DateTimeOffset? stopTime,
            bool hasWorkspaceId, long? workspaceId,
            bool hasProjectId, long? projectId,
            bool hasTaskId, long? taskId,
            bool hasTagIds, long[] tagIds,
            bool hasIsBillable, bool? isBillable)
            => new DeeplinkUpdateTimeEntryParameters(
                timeEntryId,
                hasDescription, description,
                hasStartTime, startTime,
                hasStopTime, stopTime,
                hasWorkspaceId, workspaceId,
                hasProjectId, projectId,
                hasTaskId, taskId,
                hasTagIds, tagIds,
                hasIsBillable, isBillable);

        public static DeeplinkParameters WithNewTimeEntry(
            string description,
            DateTimeOffset? startTime,
            DateTimeOffset? stopTime,
            TimeSpan? duration,
            long? workspaceId,
            long? projectId,
            long[] tagIds)
            => new DeeplinkNewTimeEntryParameters(description, startTime, stopTime, duration, workspaceId, projectId, tagIds);

        public static DeeplinkParameters WithEditTimeEntry(long timeEntryId)
            => new DeeplinkEditTimeEntryParameters(timeEntryId);

        public static DeeplinkParameters WithReports(long? workspaceId, DateTimeOffset? startDate, DateTimeOffset? endDate)
            => new DeeplinkShowReportsParameters(workspaceId, startDate, endDate);

        public static DeeplinkParameters WithCalendar(string eventId)
            => new DeeplinkShowCalendarParameters(eventId);

        public void Match(
            Action<DeeplinkStartTimeEntryParameters> matchStartTimeEntry,
            Action<DeeplinkContinueTimeEntryParameters> matchContinueLast,
            Action<DeeplinkStopTimeEntryParameters> matchStopTimeEntry,
            Action<DeeplinkCreateTimeEntryParameters> matchCreateTimeEntry,
            Action<DeeplinkUpdateTimeEntryParameters> matchUpdateTimeEntry,
            Action<DeeplinkNewTimeEntryParameters> matchNewTimeEntry,
            Action<DeeplinkEditTimeEntryParameters> matchEditTimeEntry,
            Action<DeeplinkShowReportsParameters> matchShowReports,
            Action<DeeplinkShowCalendarParameters> matchShowCalendar,
            Action<DeeplinkParameters> matchOther = null)
        {
            switch (this)
            {
                case DeeplinkStartTimeEntryParameters startTimeEntryParameters:
                    matchStartTimeEntry(startTimeEntryParameters);
                    return;
                case DeeplinkContinueTimeEntryParameters continueTimeEntryParameters:
                    matchContinueLast(continueTimeEntryParameters);
                    return;
                case DeeplinkStopTimeEntryParameters stopTimeEntryParameters:
                    matchStopTimeEntry(stopTimeEntryParameters);
                    return;
                case DeeplinkCreateTimeEntryParameters createTimeEntryParameters:
                    matchCreateTimeEntry(createTimeEntryParameters);
                    return;
                case DeeplinkUpdateTimeEntryParameters updateTimeEntryParameters:
                    matchUpdateTimeEntry(updateTimeEntryParameters);
                    return;
                case DeeplinkNewTimeEntryParameters newTimeEntryParameters:
                    matchNewTimeEntry(newTimeEntryParameters);
                    return;
                case DeeplinkEditTimeEntryParameters editTimeEntryParameters:
                    matchEditTimeEntry(editTimeEntryParameters);
                    return;
                case DeeplinkShowReportsParameters showReportsParameters:
                    matchShowReports(showReportsParameters);
                    return;
                case DeeplinkShowCalendarParameters showCalendarParameters:
                    matchShowCalendar(showCalendarParameters);
                    return;
            }

            matchOther?.Invoke(this);
        }
    }
}
