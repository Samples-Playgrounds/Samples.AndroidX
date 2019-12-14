using System.Threading.Tasks;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients.Interfaces
{
    public interface ILocationApi
    {
        Task<ILocation> Get();
    }
}
