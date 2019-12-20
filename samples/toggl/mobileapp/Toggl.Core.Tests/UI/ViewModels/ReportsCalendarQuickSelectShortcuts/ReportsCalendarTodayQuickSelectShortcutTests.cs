using System;
using Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts;

namespace Toggl.Core.Tests.UI.ViewModels.ReportsCalendarQuickSelectShortcuts
{
    public sealed class ReportsCalendarTodayQuickSelectShortcutTests
        : BaseReportsCalendarQuickSelectShortcutTests<ReportsCalendarTodayQuickSelectShortcut>
    {
        protected override DateTimeOffset CurrentTime => new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero);
        protected override DateTime ExpectedStart => new DateTime(2020, 1, 2);
        protected override DateTime ExpectedEnd => new DateTime(2020, 1, 2);

        protected override ReportsCalendarTodayQuickSelectShortcut CreateQuickSelectShortcut()
            => new ReportsCalendarTodayQuickSelectShortcut(TimeService);

        protected override ReportsCalendarTodayQuickSelectShortcut TryToCreateQuickSelectShortCutWithNull()
            => new ReportsCalendarTodayQuickSelectShortcut(null);
    }
}
