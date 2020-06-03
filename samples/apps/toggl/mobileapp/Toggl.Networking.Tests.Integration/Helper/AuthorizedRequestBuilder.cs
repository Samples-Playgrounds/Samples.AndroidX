using System.Net.Http;
using System.Net.Http.Headers;
using Toggl.Networking.Network;

namespace Toggl.Networking.Tests.Integration.Helper
{
    public static class AuthorizedRequestBuilder
    {
        public static HttpRequestMessage CreateRequest(Credentials credentials, string endPoint, HttpMethod method)
        {
            var requestMessage = new HttpRequestMessage(method, endPoint);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials.Header.Value);

            return requestMessage;
        }
    }
}
