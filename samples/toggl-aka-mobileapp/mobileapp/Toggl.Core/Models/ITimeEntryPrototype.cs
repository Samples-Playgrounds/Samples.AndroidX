using System;

namespace Toggl.Core.Models
{
    public interface ITimeEntryPrototype
    {
        long? ProjectId { get; }

        long? TaskId { get; }

        string Description { get; }

        long WorkspaceId { get; }

        long[] TagIds { get; }

        bool IsBillable { get; }

        DateTimeOffset StartTime { get; }

        TimeSpan? Duration { get; }
    }
}
