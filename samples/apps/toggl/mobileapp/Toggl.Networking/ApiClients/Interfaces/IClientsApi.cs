using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    public interface IClientsApi
        : IPullingApiClient<IClient>,
          IPullingChangedApiClient<IClient>,
          ICreatingApiClient<IClient>
    {
    }
}
