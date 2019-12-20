using System.Collections.Generic;
using Toggl.Shared.Models;

namespace Toggl.Storage.Models
{
    public interface IDatabaseProject : IProject, IDatabaseSyncable, IPotentiallyInaccessible
    {
        IDatabaseClient Client { get; }

        IDatabaseWorkspace Workspace { get; }

        IEnumerable<IDatabaseTask> Tasks { get; }
    }
}
