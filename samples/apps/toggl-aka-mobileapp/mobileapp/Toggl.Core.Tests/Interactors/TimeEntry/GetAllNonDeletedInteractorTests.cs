using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Toggl.Storage.Models;
using Xunit;

namespace Toggl.Core.Tests.Interactors
{
    public sealed class GetAllNonDeletedInteractorTests
    {
        public sealed class TheExecuteMethod : BaseInteractorTests
        {
            private static readonly IEnumerable<IThreadSafeTimeEntry> timeEntries = new IThreadSafeTimeEntry[]
            {
                new MockTimeEntry { Id = 0, IsDeleted = true, Workspace = new MockWorkspace { IsInaccessible = false } },
                new MockTimeEntry { Id = 1, IsDeleted = false, Workspace = new MockWorkspace { IsInaccessible = false } },
                new MockTimeEntry { Id = 2, IsDeleted = true, Workspace = new MockWorkspace { IsInaccessible = false } },
                new MockTimeEntry { Id = 4, IsDeleted = false, Workspace = new MockWorkspace { IsInaccessible = false } },
                new MockTimeEntry { Id = 5, IsDeleted = false, Workspace = new MockWorkspace { IsInaccessible = false } },
                new MockTimeEntry { Id = -2, IsDeleted = false, Workspace = new MockWorkspace { IsInaccessible = false } },
                new MockTimeEntry { Id = -3, IsDeleted = false, Workspace = new MockWorkspace { IsInaccessible = true } },
                new MockTimeEntry { Id = 6, IsDeleted = false, Workspace = new MockWorkspace { IsInaccessible = true } }
            };

            [Fact]
            public async Task RemovesAllDeletedTimeEntries()
            {
                DataSource.TimeEntries.GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>(), true)
                    .Returns(callInfo => Observable.Return(timeEntries.Where<IThreadSafeTimeEntry>(
                        callInfo.Arg<Func<IDatabaseTimeEntry, bool>>())));

                var finteredTimeEntries = await InteractorFactory.GetAllTimeEntriesVisibleToTheUser().Execute();

                finteredTimeEntries.Should().HaveCount(5);
                finteredTimeEntries.Select(te => te.IsDeleted).Should().BeEquivalentTo(new[] { false, false, false, false, false });
            }

            [Fact]
            public async Task RemovesInaccessibleTimeEntriesThatAreOwnedByTheWorkspace()
            {
                DataSource.TimeEntries.GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>(), true)
                    .Returns(callInfo => Observable.Return(timeEntries.Where<IThreadSafeTimeEntry>(
                        callInfo.Arg<Func<IDatabaseTimeEntry, bool>>())));

                var finteredTimeEntries = await InteractorFactory.GetAllTimeEntriesVisibleToTheUser().Execute();

                finteredTimeEntries.Should().HaveCount(5);
                finteredTimeEntries.Select(te => te.IsDeleted).Should().BeEquivalentTo(new[] { false, false, false, false, false });
                finteredTimeEntries.Select(te => te.IsInaccessible).Should().BeEquivalentTo(new[] { false, false, false, false, true });
            }

            [Fact]
            public void ThrowsIfDataSourceThrows()
            {
                DataSource.TimeEntries.GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>(), true)
                    .Returns(Observable.Throw<IEnumerable<IThreadSafeTimeEntry>>(new Exception()));

                Action tryGetAll = () => InteractorFactory.GetAllTimeEntriesVisibleToTheUser().Execute().Wait();

                tryGetAll.Should().Throw<Exception>();
            }
        }
    }
}