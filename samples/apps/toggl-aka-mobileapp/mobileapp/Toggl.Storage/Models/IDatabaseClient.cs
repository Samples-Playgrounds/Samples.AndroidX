using Toggl.Shared.Models;

namespace Toggl.Storage.Models
{
    public interface IDatabaseClient : IClient, IDatabaseSyncable, IPotentiallyInaccessible
    {
        IDatabaseWorkspace Workspace { get; }
    }
}
