using System;
namespace Toggl.Shared.Models.Reports
{
    public interface ITimeEntriesTotals
    {
        DateTimeOffset StartDate { get; }

        DateTimeOffset EndDate { get; }

        Resolution Resolution { get; }

        ITimeEntriesTotalsGroup[] Groups { get; }
    }
}
