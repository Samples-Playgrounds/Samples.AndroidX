using System.Collections.Generic;
using Toggl.Shared.Models;

namespace Toggl.Storage.Models
{
    public interface IDatabaseTimeEntry : ITimeEntry, IDatabaseSyncable, IPotentiallyInaccessible
    {
        IDatabaseTask Task { get; }

        IDatabaseUser User { get; }

        IDatabaseProject Project { get; }

        IDatabaseWorkspace Workspace { get; }

        IEnumerable<IDatabaseTag> Tags { get; }
    }
}
