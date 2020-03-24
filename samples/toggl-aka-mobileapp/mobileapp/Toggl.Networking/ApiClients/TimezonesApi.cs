using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;

namespace Toggl.Networking.ApiClients
{
    internal sealed class TimezonesApi : BaseApi, ITimeZonesApi
    {
        private readonly TimezoneEndpoints endPoints;

        public TimezonesApi(Endpoints endPoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credentials)
            : base(apiClient, serializer, credentials, endPoints.LoggedIn)
        {
            this.endPoints = endPoints.Timezones;
        }

        public Task<List<string>> GetAll()
            => SendRequest<string, string>(endPoints.Get, AuthHeader);
    }
}
