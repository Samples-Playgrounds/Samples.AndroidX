using System;
using System.Threading.Tasks;
using Toggl.Shared.Models.Reports;

namespace Toggl.Networking.ApiClients.Interfaces
{
    public interface ITimeEntriesReportsApi
    {
        Task<ITimeEntriesTotals> GetTotals(
            long userId, long workspaceId, DateTimeOffset startDate, DateTimeOffset endDate);
    }
}
