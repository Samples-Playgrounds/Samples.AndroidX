using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class LogItemViewModelTests
    {
        private static readonly IThreadSafeWorkspace workspace = new MockWorkspace { Id = 1 };

        public abstract class LogItemViewModelTest : BaseTest
        {
            protected MockProject Project { get; } = new MockProject { Id = 1, Name = "Project1", Color = "#123456" };
            protected IThreadSafeTimeEntry MockTimeEntry { get; } = Substitute.For<IThreadSafeTimeEntry>();

            protected Subject<DateTimeOffset> TickSubject { get; } = new Subject<DateTimeOffset>();

            protected LogItemViewModelTest()
            {
                var observable = TickSubject.AsObservable().Publish();
                observable.Connect();
                TimeService.CurrentDateTimeObservable.Returns(observable);
            }
        }

        public sealed class TheHasProjectProperty : LogItemViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public void ChecksIfTheTimeEntryProvidedHasANonNullProject(bool hasProject)
            {
                MockTimeEntry.Duration.Returns((long)TimeSpan.FromHours(1).TotalSeconds);
                MockTimeEntry.Project.Returns(hasProject ? Project : null);

                var viewModel = toViewModel(MockTimeEntry);

                viewModel.HasProject.Should().Be(hasProject);
            }
        }

        public sealed class TheDisplayName : LogItemViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public void ChecksDisplayNameContainsArchived(bool active)
            {
                Project.Active = active;
                MockTimeEntry.Duration.Returns((long)TimeSpan.FromHours(1).TotalSeconds);
                MockTimeEntry.Project.Returns(Project);

                var viewModel = toViewModel(MockTimeEntry);

                viewModel.ProjectName.Contains("(archived)").Should().Be(!active);
            }
        }

        public sealed class TheRepresentedTimeEntriesIdsProperty : LogItemViewModelTest
        {
            [Property, LogIfTooSlow]
            public void HasTimeEntriesIdsInAscendingOrder(NonEmptyArray<long> ids)
            {
                var timeEntries = ids.Get.Select(timeEntry).ToArray();

                var viewModel = new LogItemViewModel(
                    new GroupId(timeEntries.First()),
                    timeEntries.Select(te => te.Id).ToArray(),
                    LogItemVisualizationIntent.CollapsedGroupHeader,
                    false,
                    false,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    false,
                    false,
                    false,
                    false,
                    0,
                    0,
                    0,
                    false,
                    false);

                viewModel.RepresentedTimeEntriesIds.Should().BeInAscendingOrder();
            }

            [Property, LogIfTooSlow]
            public void RemembersAllIdsOfTheTimeEntries(NonEmptyArray<long> ids)
            {
                var timeEntries = ids.Get.Select(timeEntry).ToArray();

                var viewModel = new LogItemViewModel(
                    new GroupId(timeEntries.First()),
                    timeEntries.Select(te => te.Id).ToArray(),
                    LogItemVisualizationIntent.CollapsedGroupHeader,
                    false,
                    false,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    false,
                    false,
                    false,
                    false,
                    0,
                    0,
                    0,
                    false,
                    false);

                viewModel.RepresentedTimeEntriesIds.Should().BeEquivalentTo(ids.Get);
            }
        }

        private static IThreadSafeTimeEntry timeEntry(long id)
            => new MockTimeEntry { Id = id, Workspace = workspace, TagIds = Array.Empty<long>() };

        private static LogItemViewModel toViewModel(IThreadSafeTimeEntry timeEntry)
            => timeEntry.ToViewModel(
                new GroupId(timeEntry), LogItemVisualizationIntent.SingleItem, DurationFormat.Improved, 0, 0, 0);
    }
}
