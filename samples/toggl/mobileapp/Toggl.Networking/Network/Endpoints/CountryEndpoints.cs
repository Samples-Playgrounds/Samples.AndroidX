using System;

namespace Toggl.Networking.Network
{
    internal struct CountryEndpoints
    {
        private readonly Uri baseUrl;

        public CountryEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint Get => Endpoint.Get(baseUrl, "countries");
    }
}
