using System;
using Toggl.Core.UI.Interfaces;
using Toggl.Core.UI.Parameters;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.ReportsCalendar
{
    [Preserve(AllMembers = true)]
    public sealed class ReportsCalendarDayViewModel : IDiffableByIdentifier<ReportsCalendarDayViewModel>
    {
        public int Day { get; }

        public CalendarMonth CalendarMonth { get; }

        public bool IsInCurrentMonth { get; }

        public bool IsToday { get; }

        public DateTimeOffset DateTimeOffset { get; }

        public ReportsCalendarDayViewModel(int day, CalendarMonth month, bool isInCurrentMonth, DateTimeOffset today)
        {
            Day = day;
            CalendarMonth = month;
            IsInCurrentMonth = isInCurrentMonth;
            DateTimeOffset = new DateTimeOffset(month.Year, month.Month, Day, 0, 0, 0, DateTimeOffset.Now.Offset);
            IsToday = today.RoundDownToLocalDate() == DateTimeOffset.RoundDownToLocalDate();
        }

        public bool IsSelected(ReportsDateRangeParameter selectedRange)
        {
            return selectedRange != null && selectedRange.StartDate <= DateTimeOffset && selectedRange.EndDate >= DateTimeOffset;
        }

        public bool IsStartOfSelectedPeriod(ReportsDateRangeParameter selectedRange)
        {
            return selectedRange != null && selectedRange.StartDate == DateTimeOffset;
        }

        public bool IsEndOfSelectedPeriod(ReportsDateRangeParameter selectedRange)
        {
            return selectedRange != null && selectedRange.EndDate == DateTimeOffset;
        }

        public bool Equals(ReportsCalendarDayViewModel other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(other, null)) return false;

            return DateTimeOffset.Equals(other.DateTimeOffset)
                   && Day == other.Day
                   && CalendarMonth.Equals(other.CalendarMonth)
                   && IsInCurrentMonth == other.IsInCurrentMonth
                   && IsToday == other.IsToday;
        }

        public long Identifier => DateTimeOffset.GetHashCode();
    }
}
