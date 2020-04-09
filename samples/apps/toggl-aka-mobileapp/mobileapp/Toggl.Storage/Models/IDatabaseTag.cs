using Toggl.Shared.Models;

namespace Toggl.Storage.Models
{
    public interface IDatabaseTag : ITag, IDatabaseSyncable, IPotentiallyInaccessible
    {
        IDatabaseWorkspace Workspace { get; }
    }
}
