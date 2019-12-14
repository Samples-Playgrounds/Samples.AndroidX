using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.ApiClients;
using Toggl.Shared;

namespace Toggl.Core.Sync
{
    public sealed class PullingApiClientAdapter<T> : IPullingApiClient<T>
    {
        private readonly IPullingSingleApiClient<T> pullingSingleApiClient;

        public PullingApiClientAdapter(IPullingSingleApiClient<T> pullingSingleApiClient)
        {
            Ensure.Argument.IsNotNull(pullingSingleApiClient, nameof(pullingSingleApiClient));

            this.pullingSingleApiClient = pullingSingleApiClient;
        }

        public Task<List<T>> GetAll()
            => pullingSingleApiClient.Get().ContinueWith(t => new List<T> { t.Result });
    }
}
