using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Networking.ApiClients
{
    internal sealed class PushServicesApi : BaseApi, IPushServicesApi
    {
        private readonly PushServicesEndpoints endPoints;

        internal PushServicesApi(
            Endpoints endPoints,
            IApiClient apiClient,
            IJsonSerializer serializer,
            Credentials credentials)
            : base(apiClient, serializer, credentials, endPoints.LoggedIn)
        {
            this.endPoints = endPoints.PushServices;
        }

        public async Task<List<PushNotificationsToken>> GetAll()
        {
            var list = await SendRequest<List<string>>(endPoints.Get, AuthHeader).ConfigureAwait(false);
            return list.Select(token => new PushNotificationsToken(token)).ToList();
        }

        public Task Subscribe(PushNotificationsToken token)
            => SendRequest(endPoints.Subscribe, AuthHeader, json(token));

        public Task Unsubscribe(PushNotificationsToken token)
            => SendRequest(endPoints.Unsubscribe, AuthHeader, json(token));

        private string json(PushNotificationsToken token)
            => $"{{\"fcm_registration_token\": \"{token}\"}}";
    }
}
