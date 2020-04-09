using System;
using System.Threading.Tasks;

namespace Toggl.Networking.ApiClients
{
    public interface IUpdatingApiClient<T>
    {
        Task<T> Update(T entity);
    }
}
