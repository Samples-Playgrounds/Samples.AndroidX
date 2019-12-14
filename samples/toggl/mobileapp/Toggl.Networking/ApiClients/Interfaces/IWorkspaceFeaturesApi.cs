using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    public interface IWorkspaceFeaturesApi
        : IPullingApiClient<IWorkspaceFeatureCollection>
    {
    }
}
