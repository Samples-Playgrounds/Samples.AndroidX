using System;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels.TimeEntriesLog.Identity;

namespace Toggl.Core.UI.ViewModels.TimeEntriesLog
{
    public sealed class DaySummaryViewModel : IDiffable<IMainLogKey>
    {
        public string Title { get; }

        public string TotalTrackedTime { get; }

        public IMainLogKey Identity { get; }

        public DaySummaryViewModel(DateTime day, string title, string totalTrackedTime)
        {
            Title = title;
            TotalTrackedTime = totalTrackedTime;
            Identity = new DayHeaderKey(day);
        }
    }
}
