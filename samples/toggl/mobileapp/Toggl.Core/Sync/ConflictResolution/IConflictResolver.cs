using Toggl.Storage;

namespace Toggl.Core.Sync.ConflictResolution
{
    internal interface IConflictResolver<T>
    {
        ConflictResolutionMode Resolve(T localEntity, T serverEntity);
    }
}
