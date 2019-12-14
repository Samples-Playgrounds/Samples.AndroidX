using Toggl.Core.Analytics;
using Toggl.Core.Models;
using Toggl.Core.UI.Parameters;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts
{
    public sealed class ReportsCalendarThisYearQuickSelectShortcut
        : ReportsCalendarBaseQuickSelectShortcut
    {
        public ReportsCalendarThisYearQuickSelectShortcut(ITimeService timeService)
            : base(timeService, Resources.ThisYear, ReportPeriod.ThisYear)
        {
        }

        public override ReportsDateRangeParameter GetDateRange()
        {
            var start = TimeService.CurrentDateTime.RoundDownToLocalYear();
            var end = start.AddYears(1).AddDays(-1);
            return ReportsDateRangeParameter
                .WithDates(start, end)
                .WithSource(ReportsSource.ShortcutThisYear);
        }
    }
}
