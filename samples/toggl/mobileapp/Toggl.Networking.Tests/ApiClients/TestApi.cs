using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Networking.ApiClients;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared;

namespace Toggl.Networking.Tests.ApiClients
{
    internal sealed class TestApi : BaseApi
    {
        private readonly Endpoint endpoint;

        public TestApi(Endpoint endpoint, IApiClient apiClient, IJsonSerializer serializer,
            Credentials credentials, Endpoint loggedEndpoint)
            : base(apiClient, serializer, credentials, loggedEndpoint)
        {
            Ensure.Argument.IsNotNull(endpoint, nameof(endpoint));

            this.endpoint = endpoint;
        }

        public Task<T> TestCreateObservable<T>(Endpoint endpoint, IEnumerable<HttpHeader> headers,
            string body = "")
            => SendRequest<T>(endpoint, headers, body);

        public Task<string> Get()
            => SendRequest<string>(endpoint, AuthHeader);
    }
}
