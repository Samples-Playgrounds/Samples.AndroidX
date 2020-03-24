using System;
using System.Threading.Tasks;

namespace Toggl.Networking.ApiClients
{
    public interface ICreatingApiClient<T>
    {
        Task<T> Create(T entity);
    }
}
