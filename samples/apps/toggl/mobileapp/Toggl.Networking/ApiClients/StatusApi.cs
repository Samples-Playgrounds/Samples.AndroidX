using System.Linq;
using System.Threading.Tasks;
using Toggl.Networking.Helpers;
using Toggl.Networking.Network;

namespace Toggl.Networking.ApiClients
{
    internal sealed class StatusApi : IStatusApi
    {
        private readonly StatusEndpoints endpoints;
        private readonly IApiClient apiClient;

        public StatusApi(Endpoints endpoints, IApiClient apiClient)
        {
            this.endpoints = endpoints.Status;
            this.apiClient = apiClient;
        }

        public async Task IsAvailable()
        {
            var endpoint = endpoints.Get;
            var request = new Request("", endpoint.Url, Enumerable.Empty<HttpHeader>(), endpoint.Method);
            var response = await apiClient.Send(request).ConfigureAwait(false);

            if (response.IsSuccess)
                return;

            throw ApiExceptions.For(request, response);
        }
    }
}
