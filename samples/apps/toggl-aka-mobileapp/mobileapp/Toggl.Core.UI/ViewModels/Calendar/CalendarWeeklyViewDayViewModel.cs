using System;

namespace Toggl.Core.UI.ViewModels.Calendar
{
    public sealed class CalendarWeeklyViewDayViewModel
    {

        public DateTime Date { get; }

        public bool IsToday { get; }

        public bool Enabled { get; }

        public CalendarWeeklyViewDayViewModel(DateTime date, bool isToday, bool enabled)
        {
            Date = date;
            IsToday = isToday;
            Enabled = enabled;
        }
    }
}
