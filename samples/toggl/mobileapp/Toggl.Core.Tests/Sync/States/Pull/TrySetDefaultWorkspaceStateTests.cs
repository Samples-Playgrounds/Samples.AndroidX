using FluentAssertions;
using FsCheck.Xunit;
using NSubstitute;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.DataSources;
using Toggl.Core.Exceptions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync.States.Pull;
using Toggl.Core.Tests.Mocks;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Xunit;

namespace Toggl.Core.Tests.Sync.States.Pull
{
    public sealed class TrySetDefaultWorkspaceStateTests
    {
        private readonly ITogglDataSource dataSource = Substitute.For<ITogglDataSource>();
        private readonly ITimeService timeService = Substitute.For<ITimeService>();
        private readonly TrySetDefaultWorkspaceState state;

        public TrySetDefaultWorkspaceStateTests()
        {
            state = new TrySetDefaultWorkspaceState(timeService, dataSource);
            var user = new MockUser
            {
                Id = 666,
                Email = Email.From("valid@email.com"),
                BeginningOfWeek = BeginningOfWeek.Monday,
                Fullname = "Full Name",
                DefaultWorkspaceId = null
            };
            dataSource.User.Get().Returns(Observable.Return(user));
        }

        [Fact, LogIfTooSlow]
        public async Task ReturnsContinueResultIfThereIsOnlyOneWorkspaceWhenThereIsOnlyOneWorkspace()
        {
            setupWorkspaces(1);

            var transition = await state.Start();

            transition.Result.Should().Be(state.Done);
        }

        [Fact, LogIfTooSlow]
        public async Task SetsTheWorkspaceAsTheDefaultWhenThereIsOnlyOneWorkspace()
        {
            setupWorkspaces(1);

            await state.Start();

            await dataSource.User.Received().Update(
                Arg.Is<IThreadSafeUser>(user => user.DefaultWorkspaceId == 1));
        }

        [Fact, LogIfTooSlow]
        public async Task SetsSyncStatusToSyncNeededOnUserObjectWhenThereIsOnlyOneWorkspace()
        {
            setupWorkspaces(1);

            await state.Start();

            await dataSource.User.Received().Update(
                Arg.Is<IThreadSafeUser>(user => user.SyncStatus == SyncStatus.SyncNeeded));
        }

        [Fact, LogIfTooSlow]
        public void ThrowsNoDefaultWorkspaceExceptionWhenThereAreMultipleWorkspaces()
        {
            setupWorkspaces(4);

            Action tryingToStartTheState = () => state.Start().Wait();

            tryingToStartTheState.Should().Throw<NoDefaultWorkspaceException>();
        }

        [Fact, LogIfTooSlow]
        public void ThrowsNoDefaultWorkspaceExceptionWhenThereAreNoWorkspaces()
        {
            Action tryingToStartTheState = () => state.Start().Wait();

            tryingToStartTheState.Should().Throw<NoDefaultWorkspaceException>();
        }

        [Property, LogIfTooSlow]
        public void SetsAtToCurrentTime(DateTimeOffset now)
        {
            setupWorkspaces(1);
            timeService.CurrentDateTime.Returns(now);

            state.Start().Wait();

            dataSource.User.Received().Update(Arg.Is<IThreadSafeUser>(user => user.At == now)).Wait();
        }

        private void setupWorkspaces(int count)
        {
            var workspaces = Enumerable
                .Range(1, count)
                .Select(id => new MockWorkspace { Id = id })
                .Apply(Observable.Return);
            dataSource.Workspaces.GetAll().Returns(workspaces);
        }
    }
}
