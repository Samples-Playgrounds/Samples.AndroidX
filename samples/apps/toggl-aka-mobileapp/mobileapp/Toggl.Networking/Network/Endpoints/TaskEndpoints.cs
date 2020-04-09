using System;

namespace Toggl.Networking.Network
{
    internal struct TaskEndpoints
    {
        private readonly Uri baseUrl;

        public TaskEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint Get => Endpoint.Get(baseUrl, "me/tasks");

        public Endpoint GetSince(DateTimeOffset threshold)
            => Endpoint.Get(baseUrl, $"me/tasks?since={threshold.ToUnixTimeSeconds()}");

        public Endpoint Post(long workspaceId, long projectId)
            => Endpoint.Post(baseUrl, $"workspaces/{workspaceId}/projects/{projectId}/tasks");
    }
}
