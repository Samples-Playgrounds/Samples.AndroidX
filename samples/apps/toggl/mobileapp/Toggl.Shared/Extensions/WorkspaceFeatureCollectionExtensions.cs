using System.Linq;
using Toggl.Shared.Models;

namespace Toggl.Shared.Extensions
{
    public static class WorkspaceFeatureCollectionExtensions
    {
        public static bool IsEnabled(this IWorkspaceFeatureCollection self, WorkspaceFeatureId workspaceFeatureId)
            => self.Features.Any(f => f.FeatureId == workspaceFeatureId && f.Enabled);
    }
}
