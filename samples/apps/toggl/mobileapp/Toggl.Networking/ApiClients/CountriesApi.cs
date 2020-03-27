using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    internal sealed class CountriesApi : BaseApi, ICountriesApi
    {
        private readonly CountryEndpoints endPoints;

        public CountriesApi(Endpoints endPoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credentials)
            : base(apiClient, serializer, credentials, endPoints.LoggedIn)
        {
            this.endPoints = endPoints.Countries;
        }

        public Task<List<ICountry>> GetAll()
            => SendRequest<Country, ICountry>(endPoints.Get, AuthHeader);
    }
}
