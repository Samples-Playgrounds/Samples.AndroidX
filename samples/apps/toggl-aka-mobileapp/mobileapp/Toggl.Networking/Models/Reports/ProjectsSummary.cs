using System;
using System.Collections.Generic;
using Toggl.Shared;
using Toggl.Shared.Models.Reports;

namespace Toggl.Networking.Models.Reports
{
    [Preserve(AllMembers = true)]
    internal sealed class ProjectsSummary : IProjectsSummary
    {
        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public List<IProjectSummary> ProjectsSummaries { get; set; }
    }
}
