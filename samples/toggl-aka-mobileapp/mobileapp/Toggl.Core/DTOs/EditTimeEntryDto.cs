using System;
using System.Collections.Generic;
using Toggl.Shared.Extensions;

namespace Toggl.Core.DTOs
{
    public struct EditTimeEntryDto : IEquatable<EditTimeEntryDto>
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset? StopTime { get; set; }

        public long? ProjectId { get; set; }

        public long? TaskId { get; set; }

        public long WorkspaceId { get; set; }

        public bool Billable { get; set; }

        public IEnumerable<long> TagIds { get; set; }

        public bool Equals(EditTimeEntryDto other)
        {
            return Id == other.Id
                && Description == other.Description
                && StartTime == other.StartTime
                && StopTime == other.StopTime
                && ProjectId == other.ProjectId
                && TaskId == other.TaskId
                && WorkspaceId == other.WorkspaceId
                && Billable == other.Billable
                && TagIds.SetEquals(other.TagIds);
        }
    }
}
