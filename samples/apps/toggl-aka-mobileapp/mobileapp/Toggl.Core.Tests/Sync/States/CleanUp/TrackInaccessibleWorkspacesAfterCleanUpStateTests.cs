using FluentAssertions;
using NSubstitute;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync.States.CleanUp;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Storage.Models;
using Xunit;

namespace Toggl.Core.Tests.Sync.States.CleanUp
{
    public sealed class TrackInaccessibleWorkspacesAfterCleanUpStateTests
    {
        public sealed class TheConstructor
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useDataSource, bool useAnalyticsService)
            {
                Action tryingToConstructWithNulls = () => new TrackInaccessibleWorkspacesAfterCleanUpState(
                    useDataSource ? Substitute.For<ITogglDataSource>() : null,
                    useAnalyticsService ? Substitute.For<IAnalyticsService>() : null
                );

                tryingToConstructWithNulls.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheStartMethod
        {
            private readonly ITogglDataSource dataSource = Substitute.For<ITogglDataSource>();

            private IAnalyticsService analyticsService { get; } = Substitute.For<IAnalyticsService>();

            private readonly MockWorkspace inaccessibleWorkspace = new MockWorkspace { Id = 1, IsInaccessible = true };
            private readonly MockWorkspace accessibleWorkspace = new MockWorkspace { Id = 2, IsInaccessible = false };

            [Fact]
            public async Task TracksInaccessibleWorkspaces()
            {
                var workspaces = new[]
                {
                    inaccessibleWorkspace,
                    accessibleWorkspace
                };

                dataSource.Workspaces.GetAll(Arg.Any<Func<IDatabaseWorkspace, bool>>(), Arg.Is(true))
                    .Returns(callInfo =>
                    {
                        var predicate = callInfo[0] as Func<IDatabaseWorkspace, bool>;
                        var filteredWorkspace = workspaces.Where(predicate);
                        return Observable.Return(filteredWorkspace.Cast<IThreadSafeWorkspace>());
                    });

                var state = new TrackInaccessibleWorkspacesAfterCleanUpState(dataSource, analyticsService);

                var transition = await state.Start();

                analyticsService.WorkspacesInaccesibleAfterCleanUp.Received().Track(1);
            }

            [Fact]
            public async Task ReturnsOnlyOnceEvenWhenMultipleWorkspacesAreInaccessible()
            {
                var workspaces = new[]
                {
                    new MockWorkspace { Id = 1, IsInaccessible = false },
                    new MockWorkspace { Id = 2, IsInaccessible = true },
                    new MockWorkspace { Id = 3, IsInaccessible = true },
                    new MockWorkspace { Id = 4, IsInaccessible = true },
                };

                dataSource.Workspaces.GetAll(Arg.Any<Func<IDatabaseWorkspace, bool>>(), Arg.Is(true))
                    .Returns(callInfo =>
                    {
                        var predicate = callInfo[0] as Func<IDatabaseWorkspace, bool>;
                        var filteredWorkspace = workspaces.Where(predicate);
                        return Observable.Return(filteredWorkspace.Cast<IThreadSafeWorkspace>());
                    });

                var state = new TrackInaccessibleWorkspacesAfterCleanUpState(dataSource, analyticsService);
                var transition = await state.Start().SingleAsync();
                transition.Result.Should().Be(state.Done);
            }
        }
    }
}
