using System.Collections.Generic;

namespace Toggl.Shared.Models
{
    public interface IWorkspaceFeatureCollection
    {
        long WorkspaceId { get; }

        IEnumerable<IWorkspaceFeature> Features { get; }
    }
}
