using System;

namespace Toggl.Networking.Network
{
    internal struct PreferencesEndpoints
    {
        private readonly Uri baseUrl;

        public PreferencesEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint Get => Endpoint.Get(baseUrl, "me/preferences");

        public Endpoint Post => Endpoint.Post(baseUrl, "me/preferences");
    }
}
