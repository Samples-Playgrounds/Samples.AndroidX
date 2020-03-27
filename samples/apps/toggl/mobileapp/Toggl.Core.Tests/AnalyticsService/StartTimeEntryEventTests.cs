using FluentAssertions;
using FsCheck.Xunit;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Analytics;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Xunit;

namespace Toggl.Core.Tests.AnalyticsService
{
    public sealed class StartTimeEntryEventTests
    {
        public abstract class StartTimeEntryEventBaseTest
        {
            public static IEnumerable<object[]> TimeEntryStartOriginValues
                => Enum.GetValues(typeof(TimeEntryStartOrigin)).Cast<TimeEntryStartOrigin>()
                    .Select(origin => new object[] { origin });
        }

        public sealed class TheToDictionaryMethod : StartTimeEntryEventBaseTest
        {
            [Fact, LogIfTooSlow]
            public void MapsTheSevenEventParameterKeys()
            {
                StartTimeEntryEvent startTimeEntryEvent = createTimeEntryEvent();

                var eventToDictionaryResult = startTimeEntryEvent.ToDictionary();

                eventToDictionaryResult.Keys.Count.Should().Be(7);
                eventToDictionaryResult.Keys.Should()
                    .Contain(new[]
                    {
                        nameof(StartTimeEntryEvent.Origin),
                        nameof(StartTimeEntryEvent.HasEmptyDescription),
                        nameof(StartTimeEntryEvent.HasProject),
                        nameof(StartTimeEntryEvent.HasTask),
                        nameof(StartTimeEntryEvent.IsBillable),
                        nameof(StartTimeEntryEvent.IsRunning),
                        nameof(StartTimeEntryEvent.NumberOfTags)
                    });
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(TimeEntryStartOriginValues))]
            public void ConvertsTheOriginToAString(TimeEntryStartOrigin origin)
            {
                StartTimeEntryEvent startTimeEntryEvent = createTimeEntryEvent(origin);

                var eventToDictionaryResult = startTimeEntryEvent.ToDictionary();

                eventToDictionaryResult[nameof(StartTimeEntryEvent.Origin)].Should().Be(origin.ToString());
            }

            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public void ConvertsAllBooleansToStrings(bool value)
            {
                StartTimeEntryEvent startTimeEntryEvent = createTimeEntryEvent(valueForBooleans: value);

                var eventToDictionaryResult = startTimeEntryEvent.ToDictionary();

                eventToDictionaryResult[nameof(StartTimeEntryEvent.HasEmptyDescription)].Should().Be(value.ToString());
                eventToDictionaryResult[nameof(StartTimeEntryEvent.HasProject)].Should().Be(value.ToString());
                eventToDictionaryResult[nameof(StartTimeEntryEvent.HasTask)].Should().Be(value.ToString());
                eventToDictionaryResult[nameof(StartTimeEntryEvent.IsBillable)].Should().Be(value.ToString());
                eventToDictionaryResult[nameof(StartTimeEntryEvent.IsRunning)].Should().Be(value.ToString());
            }

            [Property(MaxTest = 5)]
            public void ConvertNumberOfTagsToAString(int numberOfTags)
            {
                StartTimeEntryEvent startTimeEntryEvent = createTimeEntryEvent(numberOfTags: numberOfTags);

                var eventToDictionaryResult = startTimeEntryEvent.ToDictionary();

                eventToDictionaryResult[nameof(StartTimeEntryEvent.NumberOfTags)].Should().Be(numberOfTags.ToString());
            }

            private StartTimeEntryEvent createTimeEntryEvent(TimeEntryStartOrigin origin = TimeEntryStartOrigin.SingleTimeEntryContinueButton, bool valueForBooleans = false, int numberOfTags = 0)
                => new StartTimeEntryEvent(origin, valueForBooleans, valueForBooleans, valueForBooleans, numberOfTags, valueForBooleans, valueForBooleans);
        }

        public sealed class TheWithMethod : StartTimeEntryEventBaseTest
        {
            private ITimeEntry TimeEntry { get; } = Substitute.For<ITimeEntry>();

            public TheWithMethod()
            {
                TimeEntry.Description.Returns(string.Empty);
                TimeEntry.ProjectId.Returns(default(int));
                TimeEntry.TaskId.Returns(default(int));
                TimeEntry.TagIds.Returns(new long[0].AsEnumerable());
                TimeEntry.Billable.Returns(false);
                TimeEntry.Duration.Returns(new long?());
            }

            [Theory, LogIfTooSlow]
            [InlineData("", true)]
            [InlineData(" ", true)]
            [InlineData("Stuff", false)]
            public void SetsTheHasEmptyDescriptionPropertyProperly(string description, bool isEmptyDescription)
            {
                TimeEntry.Description.Returns(description);

                StartTimeEntryEvent startTimeEntryEvent = StartTimeEntryEvent.With(default(TimeEntryStartOrigin), TimeEntry);

                startTimeEntryEvent.HasEmptyDescription.Should().Be(isEmptyDescription);
            }

            [Theory, LogIfTooSlow]
            [InlineData(1L, true)]
            [InlineData(null, false)]
            public void SetsTheHasProjectPropertyProperly(long? projectId, bool hasProject)
            {
                TimeEntry.ProjectId.Returns(projectId);

                StartTimeEntryEvent startTimeEntryEvent = StartTimeEntryEvent.With(default(TimeEntryStartOrigin), TimeEntry);

                startTimeEntryEvent.HasProject.Should().Be(hasProject);
            }

            [Theory, LogIfTooSlow]
            [InlineData(1L, true)]
            [InlineData(null, false)]
            public void SetsTheHasTaskPropertyProperly(long? projectId, bool hasTask)
            {
                TimeEntry.TaskId.Returns(projectId);

                StartTimeEntryEvent startTimeEntryEvent = StartTimeEntryEvent.With(default(TimeEntryStartOrigin), TimeEntry);

                startTimeEntryEvent.HasTask.Should().Be(hasTask);
            }

            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public void SetsTheIsBillablePropertyProperly(bool isBillable)
            {
                TimeEntry.Billable.Returns(isBillable);

                StartTimeEntryEvent startTimeEntryEvent = StartTimeEntryEvent.With(default(TimeEntryStartOrigin), TimeEntry);

                startTimeEntryEvent.IsBillable.Should().Be(isBillable);
            }

            [Theory, LogIfTooSlow]
            [InlineData(500L, false)]
            [InlineData(null, true)]
            public void SetsTheIsRunningPropertyProperly(long? duration, bool isRunning)
            {
                TimeEntry.Duration.Returns(duration);

                StartTimeEntryEvent startTimeEntryEvent = StartTimeEntryEvent.With(default(TimeEntryStartOrigin), TimeEntry);

                startTimeEntryEvent.IsRunning.Should().Be(isRunning);
            }

            [Property(StartSize = 0, MaxTest = 5)]
            public void SetsTheNumberTagsPropertyProperly(long[] tagIds)
            {
                TimeEntry.TagIds.Returns(tagIds.AsEnumerable());

                StartTimeEntryEvent startTimeEntryEvent = StartTimeEntryEvent.With(default(TimeEntryStartOrigin), TimeEntry);

                startTimeEntryEvent.NumberOfTags.Should().Be(tagIds.Length);
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(TimeEntryStartOriginValues))]
            public void SetsTheOriginPropertyProperly(TimeEntryStartOrigin origin)
            {
                StartTimeEntryEvent startTimeEntryEvent = StartTimeEntryEvent.With(origin, TimeEntry);

                startTimeEntryEvent.Origin.Should().Be(origin);
            }
        }
    }
}
