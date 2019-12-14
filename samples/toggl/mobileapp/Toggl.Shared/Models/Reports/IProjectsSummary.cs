using System;
using System.Collections.Generic;

namespace Toggl.Shared.Models.Reports
{
    public interface IProjectsSummary
    {
        DateTimeOffset StartDate { get; }
        DateTimeOffset? EndDate { get; }
        List<IProjectSummary> ProjectsSummaries { get; }
    }
}
