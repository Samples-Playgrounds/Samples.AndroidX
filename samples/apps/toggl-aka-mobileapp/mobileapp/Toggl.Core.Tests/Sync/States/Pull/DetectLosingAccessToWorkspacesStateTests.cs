using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync;
using Toggl.Core.Sync.States;
using Toggl.Core.Sync.States.Pull;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Shared.Models;
using Toggl.Storage.Models;
using Xunit;

namespace Toggl.Core.Tests.Sync.States.Pull
{
    public sealed class DetectLosingAccessToWorkspacesStateTests
    {

        public sealed class TheConstructor
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useDataSource, bool useAnalyticsService)
            {
                Action tryingToConstructWithNulls = () => new DetectLosingAccessToWorkspacesState(
                    useDataSource ? Substitute.For<IDataSource<IThreadSafeWorkspace, IDatabaseWorkspace>>() : null,
                    useAnalyticsService ? Substitute.For<IAnalyticsService>() : null
                );

                tryingToConstructWithNulls.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheStartMethod
        {
            private readonly IDataSource<IThreadSafeWorkspace, IDatabaseWorkspace> dataSource =
                Substitute.For<IDataSource<IThreadSafeWorkspace, IDatabaseWorkspace>>();

            private IAnalyticsService analyticsService { get; } = Substitute.For<IAnalyticsService>();

            private readonly IFetchObservables fetchObservables = Substitute.For<IFetchObservables>();

            [Fact]
            public async Task ReturnsFetchObservablesWhenNoAccessWasLost()
            {
                prepareDatabase(new[]
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 },
                    new MockWorkspace { Id = 3 }
                });
                prepareFetch(new List<IWorkspace>
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 },
                    new MockWorkspace { Id = 3 }
                });
                var state = new DetectLosingAccessToWorkspacesState(dataSource, analyticsService);

                var transition = await state.Start(fetchObservables);
                var parameter = ((Transition<IFetchObservables>)transition).Parameter;

                parameter.Should().Be(fetchObservables);
            }

            [Fact]
            public async Task ReturnsWorkspacesAndFetchObservableWhenAccessToSomeWorkspaceWasLost()
            {
                prepareDatabase(new[]
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 },
                    new MockWorkspace { Id = 3 }
                });
                prepareFetch(new List<IWorkspace>
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 },
                });
                var state = new DetectLosingAccessToWorkspacesState(dataSource, analyticsService);

                var transition = await state.Start(fetchObservables);
                var parameter = ((Transition<MarkWorkspacesAsInaccessibleParams>)transition).Parameter;

                parameter.Workspaces.Should().OnlyContain(workspace => workspace.Id == 3);
                parameter.FetchObservables.Should().Be(fetchObservables);
            }

            [Fact]
            public async Task IgnoresWorkspacesWhichAreStoredOnlyLocally()
            {
                prepareDatabase(new[]
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 },
                    new MockWorkspace { Id = -3 }
                });
                prepareFetch(new List<IWorkspace>
                {
                    new MockWorkspace { Id = 1 }
                });
                var state = new DetectLosingAccessToWorkspacesState(dataSource, analyticsService);

                var transition = await state.Start(fetchObservables);
                var parameter = ((Transition<MarkWorkspacesAsInaccessibleParams>)transition).Parameter;
                parameter.Workspaces.Should().OnlyContain(workspace => workspace.Id == 2);
            }

            [Fact]
            public async Task IgnoresInaccessibleWorkspaces()
            {
                prepareDatabase(new[]
                {
                    new MockWorkspace { Id = 1 },
                    new MockWorkspace { Id = 2 },
                    new MockWorkspace { Id = 3, IsInaccessible = true }
                });
                prepareFetch(new List<IWorkspace>
                {
                    new MockWorkspace { Id = 1 }
                });
                var state = new DetectLosingAccessToWorkspacesState(dataSource, analyticsService);

                var transition = await state.Start(fetchObservables);
                var parameter = ((Transition<MarkWorkspacesAsInaccessibleParams>)transition).Parameter;

                parameter.Workspaces.Should().OnlyContain(workspace => workspace.Id == 2);
            }

            private void prepareDatabase(IEnumerable<IThreadSafeWorkspace> workspaces)
            {
                dataSource.GetAll(Arg.Any<Func<IDatabaseWorkspace, bool>>())
                    .Returns(callInfo =>
                        Observable.Return(
                            workspaces.Where<IThreadSafeWorkspace>(callInfo.Arg<Func<IDatabaseWorkspace, bool>>())));
            }

            private void prepareFetch(List<IWorkspace> workspaces)
            {
                fetchObservables.GetList<IWorkspace>().Returns(Observable.Return(workspaces));
            }
        }
    }
}
