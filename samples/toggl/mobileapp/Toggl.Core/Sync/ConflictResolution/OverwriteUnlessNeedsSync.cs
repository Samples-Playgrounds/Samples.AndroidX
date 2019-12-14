using Toggl.Shared;
using Toggl.Storage;
using static Toggl.Storage.ConflictResolutionMode;

namespace Toggl.Core.Sync.ConflictResolution
{
    public class OverwriteUnlessNeedsSync<T> : IConflictResolver<T>
        where T : class, IDatabaseSyncable
    {
        public ConflictResolutionMode Resolve(T localEntity, T serverEntity)
        {
            Ensure.Argument.IsNotNull(serverEntity, nameof(serverEntity));

            if (localEntity == null)
                return Create;

            if (localEntity.SyncStatus == SyncStatus.SyncNeeded)
                return Ignore;

            return Update;
        }
    }
}
