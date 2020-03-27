using System;
using System.Reactive.Threading.Tasks;
using Toggl.Networking.ApiClients.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Models.Reports;

namespace Toggl.Core.Interactors.Reports
{
    internal sealed class GetTotalsInteractor : IInteractor<IObservable<ITimeEntriesTotals>>
    {
        private readonly ITimeEntriesReportsApi api;

        private readonly long userId;
        private readonly long workspaceId;
        private readonly DateTimeOffset startDate;
        private readonly DateTimeOffset endDate;

        public GetTotalsInteractor(
            ITimeEntriesReportsApi api,
            long userId,
            long workspaceId,
            DateTimeOffset startDate,
            DateTimeOffset endDate)
        {
            Ensure.Argument.IsNotNull(api, nameof(api));

            this.api = api;
            this.userId = userId;
            this.workspaceId = workspaceId;
            this.startDate = startDate;
            this.endDate = endDate;
        }

        public IObservable<ITimeEntriesTotals> Execute()
            => api.GetTotals(userId, workspaceId, startDate, endDate).ToObservable();
    }
}
