using Toggl.Shared.Models;

namespace Toggl.Storage.Models
{
    public interface IDatabaseWorkspace : IWorkspace, IDatabaseSyncable, IPotentiallyInaccessible { }
}
