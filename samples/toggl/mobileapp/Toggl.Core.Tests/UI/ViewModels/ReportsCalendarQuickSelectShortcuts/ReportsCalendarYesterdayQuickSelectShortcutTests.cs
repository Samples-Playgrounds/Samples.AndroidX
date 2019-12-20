﻿using System;
using Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts;

namespace Toggl.Core.Tests.UI.ViewModels.ReportsCalendarQuickSelectShortcuts
{
    public sealed class ReportsCalendarYesterdayQuickSelectShortcutTests
        : BaseReportsCalendarQuickSelectShortcutTests<ReportsCalendarYesterdayQuickSelectShortcut>
    {
        protected override DateTimeOffset CurrentTime => new DateTimeOffset(1998, 4, 5, 6, 4, 2, TimeSpan.Zero);
        protected override DateTime ExpectedStart => new DateTime(1998, 4, 4);
        protected override DateTime ExpectedEnd => new DateTime(1998, 4, 4);

        protected override ReportsCalendarYesterdayQuickSelectShortcut CreateQuickSelectShortcut()
            => new ReportsCalendarYesterdayQuickSelectShortcut(TimeService);

        protected override ReportsCalendarYesterdayQuickSelectShortcut TryToCreateQuickSelectShortCutWithNull()
            => new ReportsCalendarYesterdayQuickSelectShortcut(null);
    }
}
