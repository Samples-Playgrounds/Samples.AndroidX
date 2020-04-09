using FluentAssertions;
using NSubstitute;
using System;
using Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels.ReportsCalendarQuickSelectShortcuts
{
    public abstract class BaseReportsCalendarQuickSelectShortcutTests<T> : BaseTest
        where T : ReportsCalendarBaseQuickSelectShortcut
    {
        protected BaseReportsCalendarQuickSelectShortcutTests()
        {
            TimeService.CurrentDateTime.Returns(CurrentTime);
        }

        protected abstract T CreateQuickSelectShortcut();
        protected abstract T TryToCreateQuickSelectShortCutWithNull();

        protected abstract DateTimeOffset CurrentTime { get; }
        protected abstract DateTime ExpectedStart { get; }
        protected abstract DateTime ExpectedEnd { get; }

        [Fact, LogIfTooSlow]
        public void SetsSelectedToTrueWhenReceivesOnDateRangeChangedWithOwnDateRange()
        {
            var quickSelectShortCut = CreateQuickSelectShortcut();
            var dateRange = quickSelectShortCut.GetDateRange();

            var result = quickSelectShortCut.IsSelected(dateRange);

            result.Should().BeTrue();
        }

        [Fact, LogIfTooSlow]
        public void TheGetDateRangeReturnsExpectedDateRange()
        {
            var dateRange = CreateQuickSelectShortcut().GetDateRange();

            dateRange.StartDate.Date.Should().Be(ExpectedStart);
            dateRange.EndDate.Date.Should().Be(ExpectedEnd);
        }

        [Fact, LogIfTooSlow]
        public void ConstructorThrowsWhenTryingToConstructWithNull()
        {
            Action tryingToConstructWithNull =
                () => TryToCreateQuickSelectShortCutWithNull();

            tryingToConstructWithNull.Should().Throw<ArgumentNullException>();
        }
    }
}
