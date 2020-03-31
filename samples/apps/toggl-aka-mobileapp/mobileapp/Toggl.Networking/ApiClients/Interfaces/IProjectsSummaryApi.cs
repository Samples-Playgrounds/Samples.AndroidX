using System;
using System.Threading.Tasks;
using Toggl.Shared.Models.Reports;

namespace Toggl.Networking.ApiClients
{
    public interface IProjectsSummaryApi
    {
        Task<IProjectsSummary> GetByWorkspace(long workspaceId, DateTimeOffset startDate, DateTimeOffset? endDate);
    }
}
