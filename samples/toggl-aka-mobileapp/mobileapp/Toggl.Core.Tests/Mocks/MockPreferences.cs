using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage;

namespace Toggl.Core.Tests.Mocks
{
    public sealed class MockPreferences : IThreadSafePreferences
    {
        public TimeFormat TimeOfDayFormat { get; set; }

        public DateFormat DateFormat { get; set; }

        public DurationFormat DurationFormat { get; set; }

        public bool CollapseTimeEntries { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string LastSyncErrorMessage { get; set; }

        public bool IsDeleted { get; set; }

        public long Id { get; set; }
    }
}
