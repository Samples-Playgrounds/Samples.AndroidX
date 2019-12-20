using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    public interface ITagsApi
        : IPullingApiClient<ITag>,
          IPullingChangedApiClient<ITag>,
          ICreatingApiClient<ITag>
    {
    }
}
