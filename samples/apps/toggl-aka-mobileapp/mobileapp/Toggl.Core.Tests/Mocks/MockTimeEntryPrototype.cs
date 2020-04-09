using System;
using Toggl.Core.Models;

namespace Toggl.Core.Tests.Mocks
{
    public sealed class MockTimeEntryPrototype : ITimeEntryPrototype
    {
        public long? ProjectId { get; set; }

        public long? TaskId { get; set; }

        public string Description { get; set; }

        public long WorkspaceId { get; set; }

        public long[] TagIds { get; set; }

        public bool IsBillable { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public TimeSpan? Duration { get; set; }
    }
}
