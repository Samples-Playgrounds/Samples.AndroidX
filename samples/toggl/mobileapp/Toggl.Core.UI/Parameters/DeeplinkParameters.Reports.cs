using System;

namespace Toggl.Core.UI.Parameters
{
    public sealed class DeeplinkShowReportsParameters : DeeplinkParameters
    {
        public long? WorkspaceId { get; }

        public DateTimeOffset? StartDate { get; }

        public DateTimeOffset? EndDate { get; }

        internal DeeplinkShowReportsParameters(long? workspaceId, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            WorkspaceId = workspaceId;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
