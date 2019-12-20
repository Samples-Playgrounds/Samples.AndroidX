using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Toggl.Networking.Models.Reports;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared;
using Toggl.Shared.Models.Reports;
using Endpoints = Toggl.Networking.Network.Endpoints;
using ProjectEndpoints = Toggl.Networking.Network.Reports.ProjectEndpoints;

namespace Toggl.Networking.ApiClients
{
    internal sealed class ProjectsSummaryApi : BaseApi, IProjectsSummaryApi
    {
        private readonly ProjectEndpoints endPoints;
        private readonly IJsonSerializer serializer;
        private readonly Credentials credentials;

        public ProjectsSummaryApi(Endpoints endPoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credentials)
            : base(apiClient, serializer, credentials, endPoints.LoggedIn)
        {
            this.endPoints = endPoints.ReportsEndpoints.Projects;
            this.serializer = serializer;
            this.credentials = credentials;
        }

        public async Task<IProjectsSummary> GetByWorkspace(long workspaceId, DateTimeOffset startDate, DateTimeOffset? endDate)
        {
            var interval = endDate - startDate;
            if (interval.HasValue && interval > TimeSpan.FromDays(365))
                throw new ArgumentOutOfRangeException(nameof(endDate));

            var parameters = new ProjectsSummaryParameters(startDate, endDate);
            var json = serializer.Serialize(parameters, SerializationReason.Post);
            var endPoint = endPoints.Summary(workspaceId);
            var projectSummaries = await
                SendRequest<ProjectSummary, IProjectSummary>(endPoint, credentials.Header, json)
                    .ConfigureAwait(false);

            return new ProjectsSummary
            {
                StartDate = startDate,
                EndDate = endDate,
                ProjectsSummaries = projectSummaries
            };
        }

        [Preserve(AllMembers = true)]
        private sealed class ProjectsSummaryParameters
        {
            public string StartDate { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string EndDate { get; set; }

            public ProjectsSummaryParameters() { }

            public ProjectsSummaryParameters(DateTimeOffset startDate, DateTimeOffset? endDate)
            {
                StartDate = startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                EndDate = endDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
        }
    }
}
