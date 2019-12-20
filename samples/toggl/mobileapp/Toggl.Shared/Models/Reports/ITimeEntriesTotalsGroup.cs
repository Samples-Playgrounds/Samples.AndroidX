using System;
namespace Toggl.Shared.Models.Reports
{
    public interface ITimeEntriesTotalsGroup
    {
        TimeSpan Total { get; }

        TimeSpan Billable { get; }
    }
}
