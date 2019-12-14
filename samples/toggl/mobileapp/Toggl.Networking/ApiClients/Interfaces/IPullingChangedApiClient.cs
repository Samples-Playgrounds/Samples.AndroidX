using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toggl.Networking.ApiClients
{
    public interface IPullingChangedApiClient<T>
    {
        Task<List<T>> GetAllSince(DateTimeOffset threshold);
    }
}
