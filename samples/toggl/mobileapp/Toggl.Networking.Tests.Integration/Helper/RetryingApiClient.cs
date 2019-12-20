using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Toggl.Networking.Helpers;
using Toggl.Networking.Network;

namespace Toggl.Networking.Tests.Integration.Helper
{
    internal sealed class RetryingApiClient : IApiClient
    {
        private const int maximumNumberOfTries = 3;

        private readonly TimeSpan tooManyRequestsDelay = TimeSpan.FromSeconds(60);

        private readonly TimeSpan badGatewayDelay = TimeSpan.FromSeconds(30);

        private readonly TimeSpan maximumRandomDelay = TimeSpan.FromSeconds(10);

        private readonly IApiClient internalClient;

        private readonly Random random = new Random();

        internal RetryingApiClient(IApiClient internalClient)
        {
            this.internalClient = internalClient;
        }

        public void Dispose()
        {
            internalClient.Dispose();
        }

        public async Task<IResponse> Send(IRequest request)
        {
            var numberOfTries = 0;
            IResponse lastResponse = null;

            do
            {
                if (lastResponse != null)
                {
                    wait(lastResponse);
                }

                lastResponse = await internalClient.Send(request);

                if (!shouldRetry(lastResponse.StatusCode))
                {
                    return lastResponse;
                }
            }
            while (++numberOfTries < maximumNumberOfTries);

            Console.WriteLine($"Maximum number of retries ({maximumNumberOfTries}) exceeded.");
            throw ApiExceptions.For(request, lastResponse);
        }

        private bool shouldRetry(HttpStatusCode statusCode)
            => statusCode == (HttpStatusCode)429
               || statusCode == HttpStatusCode.BadGateway;

        private void wait(IResponse response)
        {
            var delay = this.delay(response.StatusCode);
            Console.WriteLine($"Server responded with HTTP {response.StatusCode}: {response.RawData}\n" +
                              $"Waiting for {delay.TotalSeconds} seconds and then the request will be retried.");
            Thread.Sleep(delay);
        }

        private TimeSpan delay(HttpStatusCode statusCode)
            => statusCode == (HttpStatusCode)429
                ? randomizedDelay(tooManyRequestsDelay)
                : randomizedDelay(badGatewayDelay);

        private TimeSpan randomizedDelay(TimeSpan baseDelay)
            => baseDelay + random.NextDouble() * maximumRandomDelay;

    }
}
