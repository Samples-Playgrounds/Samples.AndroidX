using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Sync.States;
using Toggl.Core.Sync.States.PullTimeEntries;
using Toggl.Core.Tests.Mocks;
using Toggl.Shared.Models;
using Toggl.Storage.Settings;
using Xunit;

namespace Toggl.Core.Tests.Sync.States.PullTimeEntries
{
    public class CreatePlaceholdersStateTests
    {
        private readonly IDataSource<IThreadSafeTestModel, IDatabaseTestModel> dataSource = Substitute.For<IDataSource<IThreadSafeTestModel, IDatabaseTestModel>>();
        private readonly IAnalyticsService analyticsService = Substitute.For<IAnalyticsService>();
        private readonly ILastTimeUsageStorage lastTimeUsageStorage = Substitute.For<ILastTimeUsageStorage>();
        private readonly ITimeService timeService = Substitute.For<ITimeService>();
        private readonly CreatePlaceholdersState<IThreadSafeTestModel, IDatabaseTestModel> state;

        public CreatePlaceholdersStateTests()
        {
            state = new CreatePlaceholdersState<IThreadSafeTestModel, IDatabaseTestModel>(
                dataSource,
                lastTimeUsageStorage,
                timeService,
                analyticsService.ProjectPlaceholdersCreated,
                te => te.ProjectId.HasValue ? new[] { te.ProjectId.Value } : new long[0], // use the nullable ProjectId of the TE
                (id, te) =>
                {
                    var testModel = Substitute.For<IThreadSafeTestModel>();
                    testModel.Id.Returns(id);
                    return testModel;
                });
        }

        [Fact]
        public async Task ReturnsSuccessResultWhenNoTimeEntriesAreFetched()
        {
            var fetchObservables = fetch();

            var transition = await state.Start(fetchObservables);

            transition.Result.Should().Be(state.Done);
        }

        [Fact]
        public async Task ReturnsSuccessResultWhenTimeEntriesDoNotHaveDependenciessOrTheDependenciesAreInTheDatabase()
        {
            var fetchObservables = fetch(new MockTimeEntry { ProjectId = null }, new MockTimeEntry { ProjectId = 123 });
            dataSource.GetAll(Arg.Any<Func<IDatabaseTestModel, bool>>())
                .Returns(Observable.Return<IEnumerable<IThreadSafeTestModel>>(new[] { new TestModel() }));

            var transition = await state.Start(fetchObservables);

            transition.Result.Should().Be(state.Done);
        }

        [Fact]
        public async Task CreatesAProjectPlaceholderIfThereIsNoProjectWithGivenIdInTheDatabase()
        {
            var timeEntry = new MockTimeEntry { ProjectId = 123, WorkspaceId = 456 };
            var fetchObservables = fetch(timeEntry);
            dataSource.GetAll(Arg.Any<Func<IDatabaseTestModel, bool>>())
                .Returns(Observable.Return<IEnumerable<IThreadSafeTestModel>>(new IThreadSafeTestModel[0]));

            await state.Start(fetchObservables);

            await dataSource.Received().Create(Arg.Is<IThreadSafeTestModel>(dto => dto.Id == timeEntry.ProjectId.Value));
        }

        [Fact]
        public async Task CreatesOnlyOnePlaceholderWhenMultipleTimeEntriesRequireSameUnknownDependency()
        {
            var projectId = 123;
            var timeEntryA = new MockTimeEntry { ProjectId = projectId, WorkspaceId = 456 };
            var timeEntryB = new MockTimeEntry { ProjectId = projectId, WorkspaceId = 456 };
            var fetchObservables = fetch(timeEntryA, timeEntryB);
            dataSource.GetByIds(Arg.Any<long[]>())
                .Returns(Observable.Return<IEnumerable<IThreadSafeTestModel>>(new IThreadSafeTestModel[0]));

            await state.Start(fetchObservables);

            await dataSource.Received(1).Create(Arg.Is<IThreadSafeTestModel>(dto => dto.Id == projectId));
        }

        [Fact]
        public async Task DoesNotCreateAPlaceholderWhenTheDependencyIsInTheDatabase()
        {
            var timeEntry = new MockTimeEntry { ProjectId = 123 };
            var fetchObservables = fetch(timeEntry);
            dataSource.GetByIds(Arg.Any<long[]>())
                .Returns(Observable.Return<IEnumerable<IThreadSafeTestModel>>(new[] { new TestModel() }));

            await state.Start(fetchObservables);

            await dataSource.DidNotReceive().Create(Arg.Any<IThreadSafeTestModel>());
        }

        [Fact]
        public async Task TracksTheNumberOfActuallyCreatedPlaceholders()
        {
            var timeEntryA = new MockTimeEntry { ProjectId = 123, WorkspaceId = 456 };
            var timeEntryB = new MockTimeEntry { ProjectId = 456, WorkspaceId = 456 };
            var timeEntryC = new MockTimeEntry { ProjectId = 789, WorkspaceId = 456 };
            var timeEntryD = new MockTimeEntry { ProjectId = null, WorkspaceId = 456 };
            var fetchObservables = fetch(timeEntryA, timeEntryB, timeEntryC, timeEntryD);
            dataSource.GetByIds(Arg.Is<long[]>(ids => ids.Length == 1 && (ids[0] == 123 || ids[0] == 456)))
                .Returns(Observable.Return<IEnumerable<IThreadSafeTestModel>>(
                    new IThreadSafeTestModel[0]));
            dataSource.GetByIds(Arg.Is<long[]>(ids => ids.Length == 0 || (ids[0] != 123 && ids[0] != 456)))
                .Returns(Observable.Return<IEnumerable<IThreadSafeTestModel>>(
                    new[] { new TestModel(), new TestModel() }));

            await state.Start(fetchObservables);

            analyticsService.ProjectPlaceholdersCreated.Received().Track(2);
        }

        [Fact]
        public async Task TracksThatNoPlaceholdersWereCreatedWhenTheResponseFromTheServerIsAnEmptyList()
        {
            var timeEntry = new MockTimeEntry { ProjectId = 123, WorkspaceId = 456 };
            var fetchObservables = fetch(timeEntry);
            dataSource.GetByIds(Arg.Any<long[]>())
                .Returns(Observable.Return<IEnumerable<IThreadSafeTestModel>>(new IThreadSafeTestModel[] { new TestModel() }));

            await state.Start(fetchObservables);

            analyticsService.ProjectPlaceholdersCreated.Received().Track(0);
        }

        [Fact]
        public async Task TracksThatNoPlaceholdersWereCreatedWhenTheProjectsAreInTheDataSource()
        {
            var fetchObservables = fetch();
            dataSource.GetAll(Arg.Any<Func<IDatabaseTestModel, bool>>())
                .Returns(Observable.Return<IEnumerable<IThreadSafeTestModel>>(
                    new IThreadSafeTestModel[0]));

            await state.Start(fetchObservables);

            analyticsService.ProjectPlaceholdersCreated.Received().Track(0);
        }

        [Fact]
        public async Task DoesNotMarkPlaceholderCreationWhenNoPlaceholdersWereCreated()
        {
            var timeEntry = new MockTimeEntry { ProjectId = 123 };
            var fetchObservables = fetch(timeEntry);
            dataSource.GetByIds(Arg.Any<long[]>())
                .Returns(Observable.Return<IEnumerable<IThreadSafeTestModel>>(new[] { new TestModel() }));

            var transition = await state.Start(fetchObservables);

            lastTimeUsageStorage.DidNotReceive().SetPlaceholdersWereCreated(Arg.Any<DateTimeOffset>());
        }

        [Fact]
        public async Task MarksPlaceholderCreationWhenNoPlaceholdersWereCreated()
        {
            var now = new DateTimeOffset(2019, 5, 21, 19, 20, 00, TimeSpan.Zero);
            timeService.CurrentDateTime.Returns(now);
            var timeEntry = new MockTimeEntry { ProjectId = 123, WorkspaceId = 456 };
            var fetchObservables = fetch(timeEntry);
            dataSource.GetAll(Arg.Any<Func<IDatabaseTestModel, bool>>())
                .Returns(Observable.Return<IEnumerable<IThreadSafeTestModel>>(new IThreadSafeTestModel[0]));

            var transition = await state.Start(fetchObservables);

            transition.Result.Should().Be(state.Done);
            lastTimeUsageStorage.Received().SetPlaceholdersWereCreated(now);
        }

        private IFetchObservables fetch(params ITimeEntry[] timeEntries)
        {
            var fetchObservables = Substitute.For<IFetchObservables>();
            fetchObservables.GetList<ITimeEntry>().Returns(Observable.Return(timeEntries.ToList()));
            return fetchObservables;
        }
    }
}
