using System;

namespace Toggl.Networking.Network
{
    internal struct ClientEndpoints
    {
        private readonly Uri baseUrl;

        public ClientEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint Get => Endpoint.Get(baseUrl, "me/clients");

        public Endpoint GetSince(DateTimeOffset threshold)
            => Endpoint.Get(baseUrl, $"me/clients?since={threshold.ToUnixTimeSeconds()}");

        public Endpoint Post(long workspaceId)
            => Endpoint.Post(baseUrl, $"workspaces/{workspaceId}/clients");
    }
}
