using Toggl.Core.Analytics;
using Toggl.Core.Models;
using Toggl.Core.UI.Parameters;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts
{
    public sealed class ReportsCalendarYesterdayQuickSelectShortcut : ReportsCalendarBaseQuickSelectShortcut
    {
        public ReportsCalendarYesterdayQuickSelectShortcut(ITimeService timeService)
           : base(timeService, Resources.Yesterday, ReportPeriod.Yesterday)
        {
        }

        public override ReportsDateRangeParameter GetDateRange()
        {
            var yesterday = TimeService.CurrentDateTime.RoundDownToLocalDate().AddDays(-1);
            return ReportsDateRangeParameter
                .WithDates(yesterday, yesterday)
                .WithSource(ReportsSource.ShortcutYesterday);
        }
    }
}
