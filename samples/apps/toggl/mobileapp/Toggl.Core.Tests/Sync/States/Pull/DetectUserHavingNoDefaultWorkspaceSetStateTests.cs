using FluentAssertions;
using NSubstitute;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Sync.States.Pull;
using Toggl.Core.Tests.Mocks;
using Xunit;

namespace Toggl.Core.Tests.Sync.States.Pull
{
    public sealed class DetectUserHavingNoDefaultWorkspaceSetStateTests
    {
        private readonly ITogglDataSource dataSource = Substitute.For<ITogglDataSource>();
        private readonly IAnalyticsService analyticsService = Substitute.For<IAnalyticsService>();
        private readonly DetectUserHavingNoDefaultWorkspaceSetState setState;

        public DetectUserHavingNoDefaultWorkspaceSetStateTests()
        {
            setState = new DetectUserHavingNoDefaultWorkspaceSetState(dataSource, analyticsService);
        }

        [Fact, LogIfTooSlow]
        public async Task ReturnsContinueStateWhenUserHasDefeaultWorkspace()
        {
            setupUserWithDefaultWorkspace(1);

            var transition = await setState.Start();

            transition.Result.Should().BeSameAs(setState.Done);
        }

        [Fact, LogIfTooSlow]
        public async Task ReturnsNoDefaultWorkspaceStateWhenUserDoesNotHaveDefeaultWorkspace()
        {
            setupUserWithDefaultWorkspace(null);

            var transition = await setState.Start();

            transition.Result.Should().BeSameAs(setState.NoDefaultWorkspaceDetected);
        }

        [Fact, LogIfTooSlow]
        public async Task TracksTheNoDefaultWorkspaceEventIfThereIsNoDefaultWorkspace()
        {
            setupUserWithDefaultWorkspace(null);

            await setState.Start();

            analyticsService.NoDefaultWorkspace.Received().Track(1);
        }

        [Fact, LogIfTooSlow]
        public async Task DoesNotTrackTheNoDefaultWorkspaceEventIfThereIsDefaultWorkspace()
        {
            setupUserWithDefaultWorkspace(666);

            await setState.Start();

            analyticsService.NoDefaultWorkspace.DidNotReceive().Track(Arg.Any<int>());
        }

        private void setupUserWithDefaultWorkspace(long? defaultWorkspaceId)
        {
            var user = new MockUser { DefaultWorkspaceId = defaultWorkspaceId };

            dataSource.User.Get().Returns(Observable.Return(user));

            var workspaces = new[] { new MockWorkspace { Id = defaultWorkspaceId ?? 123 } };
            dataSource.Workspaces.GetAll().Returns(Observable.Return(workspaces));
        }
    }
}
