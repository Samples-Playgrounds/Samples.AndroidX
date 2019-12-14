using Toggl.Core.Analytics;
using Toggl.Core.Models;
using Toggl.Core.UI.Parameters;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts
{
    public sealed class ReportsCalendarLastMonthQuickSelectShortcut
        : ReportsCalendarBaseQuickSelectShortcut
    {
        public ReportsCalendarLastMonthQuickSelectShortcut(ITimeService timeService)
            : base(timeService, Resources.LastMonth, ReportPeriod.LastMonth)
        {
        }

        public override ReportsDateRangeParameter GetDateRange()
        {
            var start = TimeService.CurrentDateTime.RoundDownToLocalMonth().AddMonths(-1);
            var end = start.AddMonths(1).AddDays(-1);
            return ReportsDateRangeParameter
                .WithDates(start, end)
                .WithSource(ReportsSource.ShortcutLastMonth);
        }
    }
}
