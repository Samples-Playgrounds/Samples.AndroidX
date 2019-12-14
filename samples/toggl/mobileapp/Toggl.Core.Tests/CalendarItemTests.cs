using FluentAssertions;
using FsCheck.Xunit;
using System;
using Toggl.Core.Calendar;
using Toggl.Core.Helper;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.UI.Extensions;
using Toggl.Shared;
using Toggl.Storage;
using Xunit;

namespace Toggl.Core.Tests
{
    public sealed class CalendarItemTests
    {
        public sealed class TheFromTimeEntryMethod
        {
            [Theory, LogIfTooSlow]
            [InlineData(SyncStatus.InSync, CalendarIconKind.None)]
            [InlineData(SyncStatus.RefetchingNeeded, CalendarIconKind.None)]
            [InlineData(SyncStatus.SyncFailed, CalendarIconKind.Unsyncable)]
            [InlineData(SyncStatus.SyncNeeded, CalendarIconKind.Unsynced)]
            public void SetsTheAppropriateCalendarIconKind(SyncStatus syncStatus, CalendarIconKind expectedCalendarIcon)
            {
                var timeEntry = new MockTimeEntry { SyncStatus = syncStatus };

                var calendarItem = CalendarItem.From(timeEntry);

                calendarItem.IconKind.Should().Be(expectedCalendarIcon);
            }
        }

        public sealed class TheDescriptionProperty
        {
            [Theory, LogIfTooSlow]
            [InlineData(null)]
            [InlineData("")]
            public void ShouldUseTheDefaultValueIfNullOrEmpty(string description)
            {
                var startTime = new DateTimeOffset(2018, 8, 22, 12, 0, 0, TimeSpan.Zero);
                var duration = TimeSpan.FromMinutes(30);

                var calendarItem = new CalendarItem("id", CalendarItemSource.Calendar, startTime, duration, description, CalendarIconKind.None);
                calendarItem.Description.Should().Be(Resources.NoDescription);
            }
        }

        public sealed class TheColorProperty
        {
            [Fact, LogIfTooSlow]
            public void ShouldHaveADefaultValue()
            {
                var timeEntry = new MockTimeEntry();

                var calendarItem = CalendarItem.From(timeEntry);

                calendarItem.Color.Should().Be(Colors.NoProject);
            }

            [Fact, LogIfTooSlow]
            public void ShouldInheritFromTimeEntryProjectColor()
            {
                var timeEntry = new MockTimeEntry { Project = new MockProject { Color = "#666666" } };

                var calendarItem = CalendarItem.From(timeEntry);

                calendarItem.Color.Should().Be("#666666");
            }

            [Fact, LogIfTooSlow]
            public void ShouldUseDefaultValueIfPassedAnInvalidColor()
            {
                var timeEntry = new MockTimeEntry { Project = new MockProject { Color = "#fa" } };

                var calendarItem = CalendarItem.From(timeEntry);

                calendarItem.Color.Should().Be(Colors.NoProject);
            }

            [Theory, LogIfTooSlow]
            [InlineData(Colors.NoProject, "#FFFFFF")]
            [InlineData("#000000", "#FFFFFF")]
            [InlineData("#222222", "#FFFFFF")]
            [InlineData("#FFFFFF", "#000000")]
            [InlineData("#06AAF5", "#FFFFFF")]
            [InlineData("#C56BFF", "#FFFFFF")]
            [InlineData("#EA468D", "#FFFFFF")]
            [InlineData("#FB8B14", "#FFFFFF")]
            [InlineData("#C7741C", "#FFFFFF")]
            [InlineData("#F1C33F", "#000000")]
            [InlineData("#E20505", "#FFFFFF")]
            [InlineData("#4BC800", "#FFFFFF")]
            [InlineData("#04BB9B", "#FFFFFF")]
            [InlineData("#E19A86", "#FFFFFF")]
            [InlineData("#3750B5", "#FFFFFF")]
            [InlineData("#A01AA5", "#FFFFFF")]
            [InlineData("#205500", "#FFFFFF")]
            [InlineData("#FFEEDD", "#000000")]
            public void ShouldReturnTheExpectedForegroundColor(string color, string expectedColor)
            {
                var timeEntry = new MockTimeEntry { Project = new MockProject { Color = color } };

                var calendarItem = CalendarItem.From(timeEntry);
                var foregroundColor = calendarItem.ForegroundColor();

                foregroundColor.Should().BeEquivalentTo(new Color(expectedColor));
            }
        }

        public sealed class TheDurationProperty
        {
            [Property]
            public void ShouldBeTakenFromTimeEntry(long duration)
            {
                var timeEntry = new MockTimeEntry { Duration = duration };

                var calendarItem = CalendarItem.From(timeEntry);

                calendarItem.Duration.Should().Be(TimeSpan.FromSeconds(duration));
            }
        }
    }
}
