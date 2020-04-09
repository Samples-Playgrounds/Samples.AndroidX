using System;
using Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts;

namespace Toggl.Core.Tests.UI.ViewModels.ReportsCalendarQuickSelectShortcuts
{
    public sealed class ReportsCalendarLastYearQuickSelectShortcutTests
        : BaseReportsCalendarQuickSelectShortcutTests<ReportsCalendarLastYearQuickSelectShortcut>
    {
        protected override DateTimeOffset CurrentTime => new DateTimeOffset(1984, 4, 5, 6, 7, 8, TimeSpan.Zero);
        protected override DateTime ExpectedStart => new DateTime(1983, 1, 1);
        protected override DateTime ExpectedEnd => new DateTime(1983, 12, 31);

        protected override ReportsCalendarLastYearQuickSelectShortcut CreateQuickSelectShortcut()
            => new ReportsCalendarLastYearQuickSelectShortcut(TimeService);

        protected override ReportsCalendarLastYearQuickSelectShortcut TryToCreateQuickSelectShortCutWithNull()
            => new ReportsCalendarLastYearQuickSelectShortcut(null);
    }
}
