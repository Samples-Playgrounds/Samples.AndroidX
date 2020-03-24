using FluentAssertions;
using NSubstitute;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Storage;
using Toggl.Storage.Models;
using Xunit;

namespace Toggl.Core.Tests.Interactors
{
    public sealed class ContinueMostRecentTimeEntryInteractorTests
    {
        public sealed class TheConstructor : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useIdProvider,
                bool useDataSource,
                bool useTimeService,
                bool useAnalyticsService,
                bool useSyncManager)
            {
                var idProvider = useIdProvider ? IdProvider : null;
                var dataSource = useDataSource ? DataSource : null;
                var timeService = useTimeService ? TimeService : null;
                var analyticsService = useAnalyticsService ? AnalyticsService : null;
                var syncManager = useSyncManager ? SyncManager : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new ContinueMostRecentTimeEntryInteractor(idProvider, timeService, dataSource, analyticsService, syncManager);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheExecuteMethod : BaseInteractorTests
        {
            private readonly ContinueMostRecentTimeEntryInteractor interactor;
            private readonly IDatabaseTimeEntry mostRecentTimeEntry;
            private readonly DateTimeOffset now;

            public TheExecuteMethod()
            {
                now = new DateTimeOffset(1973, 1, 2, 3, 4, 5, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);

                var timeEntries = Enumerable
                    .Range(0, 10)
                    .Select(i => new Mocks.MockTimeEntry
                    {
                        Id = i,
                        UserId = i + 10,
                        TaskId = i + 20,
                        Duration = i + 30,
                        ProjectId = i + 40,
                        WorkspaceId = i + 50,
                        Billable = i % 2 == 0,
                        Description = i.ToString(),
                        Start = DateTimeOffset.Now.AddHours(i),
                        TagIds = new long[] { 100, 101, 100 + i }
                    });
                mostRecentTimeEntry = timeEntries.Last();

                DataSource.TimeEntries.GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>())
                    .Returns(Observable.Return(timeEntries));

                interactor = new ContinueMostRecentTimeEntryInteractor(
                    IdProvider, TimeService, DataSource, AnalyticsService, SyncManager);
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesTimeEntryWithMostRecentTagIds()
            {
                await interactor.Execute();

                await DataSource
                    .TimeEntries
                    .Received()
                    .Create(Arg.Is<IThreadSafeTimeEntry>(
                        te => te.TagIds.SequenceEqual(mostRecentTimeEntry.TagIds)));
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesTimeEntryWithMostRecentUserId()
            {
                await interactor.Execute();

                await DataSource
                    .TimeEntries
                    .Received()
                    .Create(Arg.Is<IThreadSafeTimeEntry>(
                        te => te.UserId == mostRecentTimeEntry.UserId));
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesTimeEntryWithMostRecentTaskId()
            {
                await interactor.Execute();

                await DataSource
                    .TimeEntries
                    .Received()
                    .Create(Arg.Is<IThreadSafeTimeEntry>(
                        te => te.TaskId == mostRecentTimeEntry.TaskId));
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesTimeEntryWithMostRecentBillable()
            {
                await interactor.Execute();

                await DataSource
                    .TimeEntries
                    .Received()
                    .Create(Arg.Is<IThreadSafeTimeEntry>(
                        te => te.Billable == mostRecentTimeEntry.Billable));
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesTimeEntryWithMostRecentProjectId()
            {
                await interactor.Execute();

                await DataSource
                    .TimeEntries
                    .Received()
                    .Create(Arg.Is<IThreadSafeTimeEntry>(
                        te => te.ProjectId == mostRecentTimeEntry.ProjectId));
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesTimeEntryAtCurrentDateTime()
            {
                await interactor.Execute();

                await DataSource
                    .TimeEntries
                    .Received()
                    .Create(Arg.Is<IThreadSafeTimeEntry>(
                        te => te.At == now));
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesTimeEntryWithSyncNeeded()
            {
                await interactor.Execute();

                await DataSource
                    .TimeEntries
                    .Received()
                    .Create(Arg.Is<IThreadSafeTimeEntry>(
                        te => te.SyncStatus == SyncStatus.SyncNeeded));
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesTimeEntryWithMostRecentDescription()
            {
                await interactor.Execute();

                await DataSource
                    .TimeEntries
                    .Received()
                    .Create(Arg.Is<IThreadSafeTimeEntry>(
                        te => te.Description == mostRecentTimeEntry.Description));
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesTimeEntryWithCurrentStartTime()
            {
                await interactor.Execute();

                await DataSource
                    .TimeEntries
                    .Received()
                    .Create(Arg.Is<IThreadSafeTimeEntry>(
                        te => te.Start == now));
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesTimeEntryWithMostRecentWorkspaceId()
            {
                await interactor.Execute();

                await DataSource
                    .TimeEntries
                    .Received()
                    .Create(Arg.Is<IThreadSafeTimeEntry>(
                        te => te.WorkspaceId == mostRecentTimeEntry.WorkspaceId));
            }

            [Fact, LogIfTooSlow]
            public async Task InitiatesPushSync()
            {
                await interactor.Execute();

                await SyncManager.Received().PushSync();
            }

            [Fact, LogIfTooSlow]
            public async Task TracksStartedTimeEntry()
            {
                await interactor.Execute();

                AnalyticsService.Received().Track(Arg.Is<StartTimeEntryEvent>(
                    startTimeEntryEvent => startTimeEntryEvent.Origin == TimeEntryStartOrigin.ContinueMostRecent));
            }
        }
    }
}
