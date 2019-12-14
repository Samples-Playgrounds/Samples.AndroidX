using System.Threading.Tasks;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    public interface IWorkspacesApi
        : IPullingApiClient<IWorkspace>,
          ICreatingApiClient<IWorkspace>
    {
        Task<IWorkspace> GetById(long id);
    }
}
