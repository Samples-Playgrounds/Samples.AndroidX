using System;

namespace Toggl.Networking.Network
{
    internal struct TimeEntryEndpoints
    {
        private readonly Uri baseUrl;

        public TimeEntryEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint Get => Endpoint.Get(baseUrl, "me/time_entries");

        public Endpoint GetSince(DateTimeOffset threshold)
            => Endpoint.Get(baseUrl, $"me/time_entries?since={threshold.ToUnixTimeSeconds()}");

        public Endpoint GetBetween(DateTimeOffset start, DateTimeOffset end)
            => Endpoint.Get(baseUrl, $"me/time_entries?start_date={start:yyyy-MM-dd}&end_date={end:yyyy-MM-dd}");

        public Endpoint Post(long workspaceId)
            => Endpoint.Post(baseUrl, $"workspaces/{workspaceId}/time_entries");

        public Endpoint Put(long workspaceId, long timeEntryId)
            => Endpoint.Put(baseUrl, $"workspaces/{workspaceId}/time_entries/{timeEntryId}");

        public Endpoint Delete(long workspaceId, long timeEntryId)
            => Endpoint.Delete(baseUrl, $"workspaces/{workspaceId}/time_entries/{timeEntryId}");
    }
}
