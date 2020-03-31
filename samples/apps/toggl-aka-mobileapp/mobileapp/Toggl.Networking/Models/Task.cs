using System;
using Toggl.Shared.Models;

namespace Toggl.Networking.Models
{
    internal partial class Task : ITask
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long ProjectId { get; set; }

        public long WorkspaceId { get; set; }

        public long? UserId { get; set; }

        public long EstimatedSeconds { get; set; }

        public bool Active { get; set; }

        public DateTimeOffset At { get; set; }

        public long TrackedSeconds { get; set; }
    }
}
