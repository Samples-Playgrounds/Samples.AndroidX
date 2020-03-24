using System;
namespace Toggl.Networking.Network.Reports
{
    internal sealed class TimeEntriesEndpoints
    {
        private readonly Uri baseUrl;

        public TimeEntriesEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint Totals(long workspaceId)
            => Endpoint.Post(baseUrl, $"workspace/{workspaceId}/search/time_entries/totals");
    }
}
