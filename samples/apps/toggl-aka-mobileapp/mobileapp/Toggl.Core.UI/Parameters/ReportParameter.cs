using Toggl.Core.Models;

namespace Toggl.Core.UI.Parameters
{
    public sealed class ReportParameter
    {
        public ReportPeriod ReportPeriod { get; }
        public long? WorkspaceId { get; }

        public ReportParameter(ReportPeriod reportPeriod, long? workspaceId = null)
        {
            ReportPeriod = reportPeriod;
            WorkspaceId = workspaceId;
        }
    }
}
