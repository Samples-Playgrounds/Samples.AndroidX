using System;
using Toggl.Shared;
using Toggl.Shared.Models.Reports;

namespace Toggl.Networking.Models.Reports
{
    [Preserve(AllMembers = true)]
    public sealed class TimeEntriesTotals : ITimeEntriesTotals
    {
        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public Resolution Resolution { get; set; }

        public ITimeEntriesTotalsGroup[] Groups { get; set; }
    }
}
