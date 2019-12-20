using System;

namespace Toggl.Networking.Network.Reports
{
    internal sealed class ProjectEndpoints
    {
        private readonly Uri baseUrl;

        public ProjectEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint Summary(long workspaceId)
            => Endpoint.Post(baseUrl, $"workspace/{workspaceId}/projects/summary");

        public Endpoint Search(long workspaceId)
            => Endpoint.Post(baseUrl, $"workspace/{workspaceId}/search/projects");
    }
}
