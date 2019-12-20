using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    public interface ICountriesApi
    {
        Task<List<ICountry>> GetAll();
    }
}
