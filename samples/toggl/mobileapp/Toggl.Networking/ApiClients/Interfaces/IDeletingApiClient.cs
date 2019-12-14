using System.Threading.Tasks;

namespace Toggl.Networking.ApiClients.Interfaces
{
    public interface IDeletingApiClient<T>
    {
        Task Delete(T entity);
    }
}
