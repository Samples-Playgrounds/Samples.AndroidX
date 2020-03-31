using Toggl.Shared.Models;

namespace Toggl.Core.Models
{
    internal partial class WorkspaceFeatureCollection
    {
        public long Id => WorkspaceId;

        public static WorkspaceFeatureCollection From(IWorkspaceFeatureCollection entity)
            => new WorkspaceFeatureCollection(entity);
    }
}
