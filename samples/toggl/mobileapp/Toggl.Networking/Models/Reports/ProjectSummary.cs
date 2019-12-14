using Toggl.Shared;
using Toggl.Shared.Models.Reports;

namespace Toggl.Networking.Models.Reports
{
    [Preserve(AllMembers = true)]
    internal sealed class ProjectSummary : IProjectSummary
    {
        public long UserId { get; set; }

        public long? ProjectId { get; set; }

        public long TrackedSeconds { get; set; }

        public long? BillableSeconds { get; set; }
    }
}
