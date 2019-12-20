using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toggl.Networking.ApiClients
{
    public interface IPullingApiClient<T>
    {
        Task<List<T>> GetAll();
    }
}
