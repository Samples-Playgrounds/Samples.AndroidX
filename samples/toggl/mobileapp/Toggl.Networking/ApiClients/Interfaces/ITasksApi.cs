using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    public interface ITasksApi
        : IPullingApiClient<ITask>,
          IPullingChangedApiClient<ITask>,
          ICreatingApiClient<ITask>
    {
    }
}
