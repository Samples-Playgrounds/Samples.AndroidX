using System.Threading.Tasks;

namespace Toggl.Networking.ApiClients
{
    public interface IStatusApi
    {
        Task IsAvailable();
    }
}
