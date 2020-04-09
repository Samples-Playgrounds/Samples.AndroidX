using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Exceptions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync.States;
using Toggl.Core.Sync.States.Pull;
using Toggl.Core.Tests.Mocks;
using Xunit;

namespace Toggl.Core.Tests.Sync.States.Pull
{
    public sealed class DetectNotHavingAccessToAnyWorkspaceStateTests
    {
        private readonly IFetchObservables fetchObservables = Substitute.For<IFetchObservables>();

        private readonly ITogglDataSource dataSource = Substitute.For<ITogglDataSource>();
        private readonly IAnalyticsService analyticsService = Substitute.For<IAnalyticsService>();

        private readonly DetectNotHavingAccessToAnyWorkspaceState state;

        public DetectNotHavingAccessToAnyWorkspaceStateTests()
        {
            state = new DetectNotHavingAccessToAnyWorkspaceState(dataSource, analyticsService);
        }

        [Fact]
        public async Task ReturnsSuccessResultWhenWorkspacesArePresent()
        {
            var arrayWithWorkspace = new List<IThreadSafeWorkspace>(new[] { new MockWorkspace() });
            dataSource.Workspaces.GetAll().Returns(Observable.Return(arrayWithWorkspace));

            var transition = await state.Start(fetchObservables);

            transition.Result.Should().Be(state.Done);
        }

        [Fact]
        public async Task DoesNotTrackTheNoWorkspacesEventWhenWorkspacesArePresent()
        {
            var arrayWithWorkspace = new List<IThreadSafeWorkspace>(new[] { new MockWorkspace() });
            dataSource.Workspaces.GetAll().Returns(Observable.Return(arrayWithWorkspace));

            await state.Start(fetchObservables);

            analyticsService.NoWorkspaces.DidNotReceive().Track();
        }

        [Fact, LogIfTooSlow]
        public void ThrowsExceptionsWhenNoWorkspacesAreAvailableInTheDatabaseAfterPullingWorspaces()
        {
            var arrayWithNoWorkspace = new List<IThreadSafeWorkspace>();
            dataSource.Workspaces.GetAll().Returns(Observable.Return(arrayWithNoWorkspace));

            Func<Task> fetchWorkspaces = async () => await state.Start(fetchObservables);

            fetchWorkspaces.Should().Throw<NoWorkspaceException>();
        }

        [Fact, LogIfTooSlow]
        public async Task TracksTheNoWorkspacesEventWhenNoWorkspacesAreAvailableInTheDatabaseAfterPullingWorspaces()
        {
            var arrayWithNoWorkspace = new List<IThreadSafeWorkspace>();
            dataSource.Workspaces.GetAll().Returns(Observable.Return(arrayWithNoWorkspace));

            try
            {
                await state.Start(fetchObservables);
            }
            catch(NoWorkspaceException)
            {
                analyticsService.NoWorkspaces.Received().Track();
            }
        }
    }
}
