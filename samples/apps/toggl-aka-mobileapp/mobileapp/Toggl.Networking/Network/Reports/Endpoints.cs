using System;
using Toggl.Networking.Helpers;

namespace Toggl.Networking.Network.Reports
{
    internal sealed class Endpoints
    {
        private readonly Uri baseUrl;

        public ProjectEndpoints Projects => new ProjectEndpoints(baseUrl);

        public TimeEntriesEndpoints TimeEntries => new TimeEntriesEndpoints(baseUrl);

        public Endpoints(ApiEnvironment environment)
        {
            baseUrl = BaseUrls.ForReports(environment);
        }
    }
}
