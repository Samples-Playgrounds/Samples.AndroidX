using System;

namespace Toggl.Networking.Network
{
    internal struct TagEndpoints
    {
        private readonly Uri baseUrl;

        public TagEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint Get => Endpoint.Get(baseUrl, "me/tags");

        public Endpoint GetSince(DateTimeOffset threshold)
            => Endpoint.Get(baseUrl, $"me/tags?since={threshold.ToUnixTimeSeconds()}");

        public Endpoint Post(long workspaceId)
            => Endpoint.Post(baseUrl, $"workspaces/{workspaceId}/tags");
    }
}
