using System.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;

namespace Toggl.Core.UI.Helper
{
    public struct ContinueTimeEntryInfo
    {
        public long Id { get; }

        public ContinueTimeEntryMode ContinueMode { get; }

        public int IndexInLog { get; }

        public int DayInLog { get; }

        public int DaysInThePast { get; }

        public ContinueTimeEntryInfo(LogItemViewModel viewModel, ContinueTimeEntryMode continueMode)
        {
            Id = viewModel.RepresentedTimeEntriesIds.First();
            IndexInLog = viewModel.IndexInLog;
            DayInLog = viewModel.DayInLog;
            DaysInThePast = viewModel.DaysInThePast;
            ContinueMode = continueMode;
        }
    }
}
