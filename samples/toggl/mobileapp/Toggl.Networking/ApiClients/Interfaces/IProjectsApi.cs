using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    public interface IProjectsApi
        : IPullingApiClient<IProject>,
          IPullingChangedApiClient<IProject>,
          ICreatingApiClient<IProject>
    {
        Task<List<IProject>> Search(long workspaceId, long[] projectIds);
    }
}
