using Toggl.Core.Analytics;
using Toggl.Core.Models;
using Toggl.Core.UI.Parameters;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts
{
    public sealed class ReportsCalendarLastWeekQuickSelectShortcut
        : ReportsCalendarBaseQuickSelectShortcut
    {
        private readonly BeginningOfWeek beginningOfWeek;

        public ReportsCalendarLastWeekQuickSelectShortcut(
            ITimeService timeService, BeginningOfWeek beginningOfWeek)
            : base(timeService, Resources.LastWeek, ReportPeriod.LastWeek)
        {
            this.beginningOfWeek = beginningOfWeek;
        }

        public override ReportsDateRangeParameter GetDateRange()
        {
            var now = TimeService.CurrentDateTime.RoundDownToLocalDate();
            var difference = (now.DayOfWeek - beginningOfWeek.ToDayOfWeekEnum() + 7) % 7;
            var start = now.AddDays(-(difference + 7));
            var end = start.AddDays(6);
            return ReportsDateRangeParameter
                .WithDates(start, end)
                .WithSource(ReportsSource.ShortcutLastWeek);
        }
    }
}
