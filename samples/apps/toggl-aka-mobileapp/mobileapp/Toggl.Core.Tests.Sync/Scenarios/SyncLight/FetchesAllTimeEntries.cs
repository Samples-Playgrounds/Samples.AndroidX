using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Sync;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.Tests.Sync.Extensions;
using Toggl.Core.Tests.Sync.Helpers;
using Toggl.Core.Tests.Sync.State;
using Toggl.Storage;

namespace Toggl.Core.Tests.Sync.Scenarios.SyncLight
{
    public sealed class FetchesAllTimeEntries : ComplexSyncTest
    {
        protected override ServerState ArrangeServerState(ServerState initialServerState)
            => initialServerState
                .With(timeEntries: new[]
                {
                    new MockTimeEntry
                    {
                        Description = "A",
                        WorkspaceId = initialServerState.DefaultWorkspace.Id,
                        Start = DateTimeOffset.Now,
                        TagIds = new long[0]
                    },
                    new MockTimeEntry
                    {
                        Description = "B",
                        WorkspaceId = initialServerState.DefaultWorkspace.Id,
                        Start = DateTimeOffset.Now,
                        TagIds = new long[0]
                    },
                    new MockTimeEntry
                    {
                        Description = "C",
                        WorkspaceId = initialServerState.DefaultWorkspace.Id,
                        Start = DateTimeOffset.Now,
                        TagIds = new long[0]
                    }
                });

        protected override DatabaseState ArrangeDatabaseState(ServerState serverState)
            => new DatabaseState(
                user: serverState.User.ToSyncable(),
                preferences: serverState.Preferences.ToSyncable(),
                workspaces: serverState.Workspaces.ToSyncable());

        protected override async Task Act(ISyncManager syncManager, AppServices appServices)
        {
            var progressMonitoring = MonitorProgress(syncManager);
            await syncManager.PullTimeEntries();
            await progressMonitoring;
        }

        protected override void AssertFinalState(AppServices services, ServerState finalServerState, DatabaseState finalDatabaseState)
        {
            finalDatabaseState.Workspaces.ContainsNoPlaceholders();
            finalDatabaseState.Projects.ContainsNoPlaceholders();
            finalDatabaseState.Tasks.ContainsNoPlaceholders();
            finalDatabaseState.Tags.ContainsNoPlaceholders();

            finalDatabaseState.TimeEntries
                .Should()
                .HaveCount(3)
                .And.Contain(te => te.Description == "A")
                .And.Contain(te => te.Description == "B")
                .And.Contain(te => te.Description == "C");
        }

        private void containsNoPlaceholdersFor<T>(ISet<T> entities)
            where T : IDatabaseSyncable
        {
            entities.Where(entity => entity.SyncStatus == SyncStatus.RefetchingNeeded)
                .Should()
                .HaveCount(0);
        }
    }
}
