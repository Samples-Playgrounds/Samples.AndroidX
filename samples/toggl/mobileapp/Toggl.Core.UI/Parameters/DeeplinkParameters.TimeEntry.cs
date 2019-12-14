using System;
using Toggl.Core.Analytics;

namespace Toggl.Core.UI.Parameters
{
    public sealed class DeeplinkStartTimeEntryParameters : DeeplinkParameters
    {
        public string Description { get; }

        public DateTimeOffset? StartTime { get; }

        public long? WorkspaceId { get; }

        public long? ProjectId { get; }

        public long? TaskId { get; }

        public long[] TagIds { get; }

        public bool? IsBillable { get; }

        public TimeEntryStartOrigin? Source { get; }

        internal DeeplinkStartTimeEntryParameters(
            string description,
            DateTimeOffset? startTime,
            long? workspaceId,
            long? projectId,
            long? taskId,
            long[] tagIds,
            bool? isBillable,
            TimeEntryStartOrigin? source)
        {
            Description = description;
            StartTime = startTime;
            WorkspaceId = workspaceId;
            ProjectId = projectId;
            TaskId = taskId;
            TagIds = tagIds;
            IsBillable = isBillable;
            Source = source;
        }
    }

    public sealed class DeeplinkContinueTimeEntryParameters : DeeplinkParameters
    {
    }

    public sealed class DeeplinkStopTimeEntryParameters : DeeplinkParameters
    {
        public DateTimeOffset? StopTime { get; }

        public TimeEntryStopOrigin? Source { get; }

        internal DeeplinkStopTimeEntryParameters(DateTimeOffset? stopTime, TimeEntryStopOrigin? source)
        {

            StopTime = stopTime;
            Source = source;
        }
    }

    public sealed class DeeplinkCreateTimeEntryParameters : DeeplinkParameters
    {
        public string Description { get; }

        public DateTimeOffset? StartTime { get; }

        public DateTimeOffset? StopTime { get; }

        public TimeSpan? Duration { get; }

        public long? WorkspaceId { get; }

        public long? ProjectId { get; }

        public long? TaskId { get; }

        public long[] TagIds { get; }

        public bool? IsBillable { get; }

        public TimeEntryStartOrigin? StartSource { get; }

        internal DeeplinkCreateTimeEntryParameters(
            string description,
            DateTimeOffset? startTime,
            DateTimeOffset? stopTime,
            TimeSpan? duration,
            long? workspaceId,
            long? projectId,
            long? taskId,
            long[] tagIds,
            bool? isBillable,
            TimeEntryStartOrigin? source)
        {
            Description = description;
            StartTime = startTime;
            StopTime = stopTime;
            Duration = duration;
            WorkspaceId = workspaceId;
            ProjectId = projectId;
            TaskId = taskId;
            TagIds = tagIds;
            IsBillable = isBillable;
            StartSource = source;
        }
    }

    public sealed class DeeplinkUpdateTimeEntryParameters : DeeplinkParameters
    {
        public long TimeEntryId { get; }

        public bool HasDescription { get; }

        public string Description { get; }

        public bool HasStartTime { get; }

        public DateTimeOffset? StartTime { get; }

        public bool HasStopTime { get; }
        public DateTimeOffset? StopTime { get; }

        public bool HasWorkspaceId { get; }

        public long? WorkspaceId { get; }

        public bool HasProjectId { get; }

        public long? ProjectId { get; }

        public bool HasTaskId { get; }

        public long? TaskId { get; }

        public bool HasTagIds { get; }

        public long[] TagIds { get; }

        public bool HasIsBillable { get; }

        public bool? IsBillable { get; }

        internal DeeplinkUpdateTimeEntryParameters(long timeEntryId,
            bool hasDescription, string description,
            bool hasStartTime, DateTimeOffset? startTime,
            bool hasStopTime, DateTimeOffset? stopTime,
            bool hasWorkspaceId, long? workspaceId,
            bool hasProjectId, long? projectId,
            bool hasTaskId, long? taskId,
            bool hasTagIds, long[] tagIds,
            bool hasIsBillable, bool? isBillable)
        {
            TimeEntryId = timeEntryId;
            HasDescription = hasDescription;
            Description = description;
            HasStartTime = hasStartTime;
            StartTime = startTime;
            HasStopTime = hasStopTime;
            StopTime = stopTime;
            HasWorkspaceId = hasWorkspaceId;
            WorkspaceId = workspaceId;
            HasProjectId = hasProjectId;
            ProjectId = projectId;
            HasTaskId = hasTaskId;
            TaskId = taskId;
            HasTagIds = hasTagIds;
            TagIds = tagIds;
            HasIsBillable = hasIsBillable;
            IsBillable = isBillable;
        }
    }

    public sealed class DeeplinkNewTimeEntryParameters : DeeplinkParameters
    {
        public string Description { get; }

        public DateTimeOffset? StartTime { get; }

        public DateTimeOffset? StopTime { get; }

        public TimeSpan? Duration { get; }

        public long? WorkspaceId { get; }

        public long? ProjectId { get; }

        public long[] TagIds { get; }

        internal DeeplinkNewTimeEntryParameters(
            string description,
            DateTimeOffset? startTime,
            DateTimeOffset? stopTime,
            TimeSpan? duration,
            long? workspaceId,
            long? projectId,
            long[] tagIds)
        {
            Description = description;
            StartTime = startTime;
            StopTime = stopTime;
            Duration = duration;
            WorkspaceId = workspaceId;
            ProjectId = projectId;
            TagIds = tagIds;
        }
    }

    public sealed class DeeplinkEditTimeEntryParameters : DeeplinkParameters
    {
        public long TimeEntryId { get; }

        internal DeeplinkEditTimeEntryParameters(long timeEntryId)
        {
            TimeEntryId = timeEntryId;
        }
    }
}
