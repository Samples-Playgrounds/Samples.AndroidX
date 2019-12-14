using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toggl.Networking.ApiClients
{
    public interface ITimeZonesApi
    {
        Task<List<string>> GetAll();
    }
}
