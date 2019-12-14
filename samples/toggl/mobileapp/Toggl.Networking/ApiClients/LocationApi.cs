using System.Threading.Tasks;
using Toggl.Networking.ApiClients.Interfaces;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    internal sealed class LocationApi : BaseApi, ILocationApi
    {
        private readonly LocationEndpoints endPoints;

        public LocationApi(Endpoints endPoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credentials)
            : base(apiClient, serializer, credentials, endPoints.LoggedIn)
        {
            Ensure.Argument.IsNotNull(endPoints, nameof(endPoints));

            this.endPoints = endPoints.Location;
        }

        public async Task<ILocation> Get()
            => await SendRequest<Location>(endPoints.Get, AuthHeader)
                .ConfigureAwait(false);
    }
}
