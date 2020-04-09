using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels.TimeEntriesLog
{
    public sealed class GroupId : IEquatable<GroupId>
    {
        private readonly DateTime date;
        private readonly string description;
        private readonly long workspaceId;
        private readonly long? projectId;
        private readonly long? taskId;
        private readonly bool isBillable;
        private readonly HashSet<long> tagIds;

        public GroupId(IThreadSafeTimeEntry sample)
        {
            date = sample.Start.LocalDateTime.Date;
            description = sample.Description;
            workspaceId = sample.WorkspaceId;
            projectId = sample.Project?.Id;
            taskId = sample.Task?.Id;
            isBillable = sample.Billable;
            tagIds = new HashSet<long>(sample.TagIds ?? Array.Empty<long>());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is GroupId other && Equals(other);
        }

        public bool Equals(GroupId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return date.Equals(other.date)
                   && string.Equals(description, other.description)
                   && workspaceId == other.workspaceId
                   && projectId == other.projectId
                   && taskId == other.taskId
                   && isBillable == other.isBillable
                   && tagIds.SetEquals(other.tagIds);
        }

        public override int GetHashCode()
        {
            var hashCode = HashCode.Combine(date, description, workspaceId, projectId, taskId, isBillable);
            return tagIds.Aggregate(hashCode, HashCode.Combine);
        }
    }
}
