using Toggl.Storage.Models;

namespace Toggl.Storage
{
    public interface IDatabaseSyncable : IDatabaseModel
    {
        SyncStatus SyncStatus { get; }

        string LastSyncErrorMessage { get; }

        bool IsDeleted { get; }
    }
}
