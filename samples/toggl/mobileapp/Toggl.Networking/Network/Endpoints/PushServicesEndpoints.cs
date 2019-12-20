using System;
using Toggl.Shared.Extensions;

namespace Toggl.Networking.Network
{
    internal struct PushServicesEndpoints
    {
        private readonly Uri baseUrl;

        public PushServicesEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint Get => Endpoint.Get(baseUrl, "me/push_services");

        public Endpoint Subscribe => Endpoint.Post(baseUrl, "me/push_services");

        public Endpoint Unsubscribe => Endpoint.Delete(baseUrl, "me/push_services");
    }
}
