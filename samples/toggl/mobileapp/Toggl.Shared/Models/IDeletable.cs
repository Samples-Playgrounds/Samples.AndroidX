using System;

namespace Toggl.Shared.Models
{
    public interface IDeletable
    {
        DateTimeOffset? ServerDeletedAt { get; }
    }
}
