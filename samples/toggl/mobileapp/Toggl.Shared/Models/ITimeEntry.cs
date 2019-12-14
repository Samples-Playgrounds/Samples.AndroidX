using System;
using System.Collections.Generic;

namespace Toggl.Shared.Models
{
    public interface ITimeEntry : IIdentifiable, IDeletable, ILastChangedDatable
    {
        long WorkspaceId { get; }

        long? ProjectId { get; }

        long? TaskId { get; }

        bool Billable { get; }

        DateTimeOffset Start { get; }

        long? Duration { get; }

        string Description { get; }

        IEnumerable<long> TagIds { get; }

        long UserId { get; }
    }
}
