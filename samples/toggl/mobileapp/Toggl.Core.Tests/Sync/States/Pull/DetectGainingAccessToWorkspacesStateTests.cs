using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync;
using Toggl.Core.Sync.States;
using Toggl.Core.Sync.States.Pull;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Shared.Models;
using Toggl.Storage;
using Toggl.Storage.Models;
using Xunit;

namespace Toggl.Core.Tests.Sync.States.Pull
{
    public sealed class DetectGainingAccessToWorkspacesStateTests
    {
        public abstract class DetectGainingAccessToWorkspacesStateTestBase
        {
            protected IDataSource<IThreadSafeWorkspace, IDatabaseWorkspace> DataSource { get; } =
                Substitute.For<IDataSource<IThreadSafeWorkspace, IDatabaseWorkspace>>();

            protected IAnalyticsService AnalyticsService { get; } =
                Substitute.For<IAnalyticsService>();

            protected Func<IInteractor<IObservable<bool>>> HasFinsihedSyncBeforeInteractor { get; } =
                Substitute.For<Func<IInteractor<IObservable<bool>>>>();

            protected IFetchObservables FetchObservables { get; } = Substitute.For<IFetchObservables>();

            protected void PrepareDatabase(IEnumerable<IThreadSafeWorkspace> workspaces)
            {
                var accessibleWorkspaces = workspaces.Where(ws => !ws.IsInaccessible);
                DataSource.GetAll(Arg.Any<Func<IDatabaseWorkspace, bool>>())
                    .Returns(callInfo =>
                    {
                        var filter = callInfo.Arg<Func<IDatabaseWorkspace, bool>>();
                        var filteredWorkspaces = accessibleWorkspaces.Where<IThreadSafeWorkspace>(filter).ToList();
                        return Observable.Return(filteredWorkspaces);
                    });
            }

            protected void PrepareFetch(List<IWorkspace> workspaces)
            {
                FetchObservables.GetList<IWorkspace>().Returns(Observable.Return(workspaces));
            }
        }

        public sealed class TheConstructor : DetectGainingAccessToWorkspacesStateTestBase
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useDataSource,
                bool useAnalyticsService,
                bool useHasFinsihedSyncBeforeInteractor)
            {
                var theDataSource = useDataSource ? DataSource : null;
                var theAnalyticsService = useAnalyticsService ? AnalyticsService : null;
                var theInteractor = useHasFinsihedSyncBeforeInteractor ? HasFinsihedSyncBeforeInteractor : null;

                Action tryingToConstructAction = () => new DetectGainingAccessToWorkspacesState(
                    theDataSource,
                    theAnalyticsService,
                    theInteractor
                );

                tryingToConstructAction.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class WhenFirstSyncHasNotFinishedBefore : DetectGainingAccessToWorkspacesStateTestBase
        {
            private DetectGainingAccessToWorkspacesState state;

            public WhenFirstSyncHasNotFinishedBefore()
            {
                HasFinsihedSyncBeforeInteractor().Execute().Returns(Observable.Return(false));
                state = new DetectGainingAccessToWorkspacesState(DataSource, AnalyticsService, HasFinsihedSyncBeforeInteractor);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsFetchObservables()
            {
                PrepareDatabase(new IThreadSafeWorkspace[] { });
                PrepareFetch(new List<IWorkspace>
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 }
                });

                var transition = await state.Start(FetchObservables);
                var parameter = ((Transition<IFetchObservables>)transition).Parameter;

                parameter.Should().Be(FetchObservables);
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotTrackAnEventAfterLoginOrSignUp()
            {
                PrepareDatabase(new IThreadSafeWorkspace[] { });
                PrepareFetch(new List<IWorkspace>
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 }
                });

                await state.Start(FetchObservables).SingleAsync();

                AnalyticsService.GainWorkspaceAccess.DidNotReceive().Track();
            }
        }

        public sealed class WhenNoNewWorkspacesAreDetected : DetectGainingAccessToWorkspacesStateTestBase
        {
            private DetectGainingAccessToWorkspacesState state;

            public WhenNoNewWorkspacesAreDetected()
            {
                HasFinsihedSyncBeforeInteractor().Execute().Returns(Observable.Return(true));
                state = new DetectGainingAccessToWorkspacesState(DataSource, AnalyticsService, HasFinsihedSyncBeforeInteractor);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsFetchObservables()
            {
                PrepareDatabase(new[]
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 }
                });
                PrepareFetch(new List<IWorkspace>
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 }
                });

                var transition = await state.Start(FetchObservables);
                var parameter = ((Transition<IFetchObservables>)transition).Parameter;

                parameter.Should().Be(FetchObservables);
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotTrackNewWorkspaces()
            {
                PrepareDatabase(new[]
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 }
                });
                PrepareFetch(new List<IWorkspace>
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 }
                });

                await state.Start(FetchObservables).SingleAsync();

                AnalyticsService.GainWorkspaceAccess.DidNotReceive().Track();
            }
        }

        public sealed class WhenNewWorkspacesAreDetected : DetectGainingAccessToWorkspacesStateTestBase
        {
            private DetectGainingAccessToWorkspacesState state;

            public WhenNewWorkspacesAreDetected()
            {
                HasFinsihedSyncBeforeInteractor().Execute().Returns(Observable.Return(true));
                state = new DetectGainingAccessToWorkspacesState(DataSource, AnalyticsService, HasFinsihedSyncBeforeInteractor);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsNewWorkspacesDetected()
            {
                PrepareDatabase(new[]
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 }
                });
                PrepareFetch(new List<IWorkspace>
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 },
                    new MockWorkspace { Id = 3 },
                });

                var transition = await state.Start(FetchObservables);
                var parameter = ((Transition<IEnumerable<IWorkspace>>)transition).Parameter;

                parameter.Should().Match(newWorkspaces => newWorkspaces.All(ws => ws.Id == 3));
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsNewWorkspacesDetectedIfStoredWorkspaceIsInaccessibleAndHasSameId()
            {
                PrepareDatabase(new[]
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2, IsInaccessible = true }
                });
                PrepareFetch(new List<IWorkspace>
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 },
                });

                var transition = await state.Start(FetchObservables);
                var parameter = ((Transition<IEnumerable<IWorkspace>>)transition).Parameter;

                parameter.Should().Match(newWorkspaces => newWorkspaces.All(ws => ws.Id == 2));
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsNewWorkspacesDetectedEvenIfSomeAccessWasLostInAnotherWorkspace()
            {
                PrepareDatabase(new[]
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 }
                });
                PrepareFetch(new List<IWorkspace>
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 3 },
                });

                var transition = await state.Start(FetchObservables);
                var parameter = ((Transition<IEnumerable<IWorkspace>>)transition).Parameter;

                parameter.Should().Match(newWorkspaces => newWorkspaces.All(ws => ws.Id == 3));
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsNewWorkspacesDetectedIfStoredWorkspaceIsAPlaceholderAndHasSameId()
            {
                PrepareDatabase(new[]
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2, SyncStatus = SyncStatus.RefetchingNeeded }
                });
                PrepareFetch(new List<IWorkspace>
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 }
                });

                var transition = await state.Start(FetchObservables);
                var parameter = ((Transition<IEnumerable<IWorkspace>>)transition).Parameter;

                parameter.Should().Match(newWorkspaces => newWorkspaces.All(ws => ws.Id == 2));
            }

            [Fact, LogIfTooSlow]
            public async Task TracksNewWorkspaces()
            {
                PrepareDatabase(new[]
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 }
                });
                PrepareFetch(new List<IWorkspace>
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 3 },
                    new MockWorkspace { Id = 4 }
                });

                await state.Start(FetchObservables).SingleAsync();

                AnalyticsService.GainWorkspaceAccess.Received().Track();
            }
        }
    }
}
