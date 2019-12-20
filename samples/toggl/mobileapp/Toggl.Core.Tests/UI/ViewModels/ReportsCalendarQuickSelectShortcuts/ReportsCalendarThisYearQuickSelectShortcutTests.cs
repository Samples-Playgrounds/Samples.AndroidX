using System;
using Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts;

namespace Toggl.Core.Tests.UI.ViewModels.ReportsCalendarQuickSelectShortcuts
{
    public sealed class ReportsCalendarThisYearQuickSelectShortcutTests
        : BaseReportsCalendarQuickSelectShortcutTests<ReportsCalendarThisYearQuickSelectShortcut>
    {
        protected override DateTimeOffset CurrentTime => new DateTimeOffset(1984, 4, 5, 6, 7, 8, TimeSpan.Zero);
        protected override DateTime ExpectedStart => new DateTime(1984, 1, 1);
        protected override DateTime ExpectedEnd => new DateTime(1984, 12, 31);

        protected override ReportsCalendarThisYearQuickSelectShortcut CreateQuickSelectShortcut()
            => new ReportsCalendarThisYearQuickSelectShortcut(TimeService);

        protected override ReportsCalendarThisYearQuickSelectShortcut TryToCreateQuickSelectShortCutWithNull()
            => new ReportsCalendarThisYearQuickSelectShortcut(null);
    }
}
