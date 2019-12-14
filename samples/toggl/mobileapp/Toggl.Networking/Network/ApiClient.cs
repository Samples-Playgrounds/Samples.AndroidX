using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Toggl.Networking.Extensions;
using Toggl.Shared;

namespace Toggl.Networking.Network
{
    internal sealed class ApiClient : IApiClient
    {
        private const string defaultContentType = "text/plain";

        private readonly HttpClient httpClient;

        public ApiClient(HttpClient httpClient, UserAgent userAgent)
        {
            Ensure.Argument.IsNotNull(userAgent, nameof(userAgent));
            Ensure.Argument.IsNotNull(httpClient, nameof(httpClient));

            this.httpClient = httpClient;
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent.ToString());
        }

        public async Task<IResponse> Send(IRequest request)
        {
            Ensure.Argument.IsNotNull(request, nameof(request));

            var requestMessage = createRequestMessage(request);
            var responseMessage = await httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            var response = await createResponse(responseMessage).ConfigureAwait(false);
            return response;
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        private HttpRequestMessage createRequestMessage(IRequest request)
        {
            Ensure.Argument.IsNotNull(request, nameof(request));

            var requestMessage = new HttpRequestMessage(request.HttpMethod, request.Endpoint);
            requestMessage.Headers.AddRange(request.Headers);

            if (request.HttpMethod != HttpMethod.Get)
            {
                requestMessage.Content = request.Body.Match<HttpContent>(
                    json => new StringContent(json, Encoding.UTF8, "application/json"),
                    bytes => new ByteArrayContent(bytes)
                );
            }

            return requestMessage;
        }

        private async Task<IResponse> createResponse(HttpResponseMessage responseMessage)
        {
            Ensure.Argument.IsNotNull(responseMessage, nameof(responseMessage));

            var rawResponseString = "";
            var isSuccess = responseMessage.IsSuccessStatusCode;
            var contentType = responseMessage.Content?.Headers?.ContentType?.MediaType ?? defaultContentType;
            var headers = responseMessage.Headers;

            using (var content = responseMessage.Content)
            {
                if (content != null)
                {
                    rawResponseString = await content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }

            var response = new Response(rawResponseString, isSuccess, contentType, headers, responseMessage.StatusCode);
            return response;
        }
    }
}
