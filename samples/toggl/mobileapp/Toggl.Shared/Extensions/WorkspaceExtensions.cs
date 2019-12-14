using Toggl.Shared.Models;

namespace Toggl.Shared.Extensions
{
    public static class WorkspaceExtensions
    {
        public static bool IsEligibleForProjectCreation(this IWorkspace self) =>
            self.Admin || !self.OnlyAdminsMayCreateProjects;
    }
}