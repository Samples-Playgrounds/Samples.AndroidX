using Toggl.Shared.Models;

namespace Toggl.Storage.Models
{
    public interface IDatabaseTask : ITask, IDatabaseSyncable, IPotentiallyInaccessible
    {
        IDatabaseUser User { get; }

        IDatabaseProject Project { get; }

        IDatabaseWorkspace Workspace { get; }
    }
}
