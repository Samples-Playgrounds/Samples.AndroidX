using System.Threading.Tasks;

namespace Toggl.Networking.ApiClients
{
    public interface IPullingSingleApiClient<T>
    {
        Task<T> Get();
    }
}
