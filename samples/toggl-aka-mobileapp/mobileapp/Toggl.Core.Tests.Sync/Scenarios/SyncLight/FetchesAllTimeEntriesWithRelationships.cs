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
using Toggl.Networking.Helpers;
using Toggl.Storage;

namespace Toggl.Core.Tests.Sync.Scenarios.SyncLight
{
    public sealed class FetchesAllTimeEntriesWithRelationships : ComplexSyncTest
    {
        protected override ServerState ArrangeServerState(ServerState initialServerState)
            => initialServerState
                .With(
                    projects: new[]
                    {
                        new MockProject { Id = -1, Name = "Project", WorkspaceId = initialServerState.DefaultWorkspace.Id, Active = true, Color = "#06AAF5" }
                    },
                    tasks: new[]
                    {
                        new MockTask { Id = -2, Name = "Task", WorkspaceId = initialServerState.DefaultWorkspace.Id, ProjectId = -1, Active = true }
                    },
                    tags: new[]
                    {
                        new MockTag { Id = -3, Name = "Tag", WorkspaceId = initialServerState.DefaultWorkspace.Id }
                    },
                    timeEntries: new[]
                    {
                        new MockTimeEntry
                        {
                            Description = "A",
                            WorkspaceId = initialServerState.DefaultWorkspace.Id,
                            Start = DateTimeOffset.Now,
                            TagIds = new long[0],
                            ProjectId = -1
                        },
                        new MockTimeEntry
                        {
                            Description = "B",
                            WorkspaceId = initialServerState.DefaultWorkspace.Id,
                            Start = DateTimeOffset.Now,
                            TagIds = new long[0],
                            ProjectId = -1,
                            TaskId = -2
                        },
                        new MockTimeEntry
                        {
                            Description = "C",
                            WorkspaceId = initialServerState.DefaultWorkspace.Id,
                            Start = DateTimeOffset.Now,
                            TagIds = new long[] { -3 }
                        }
                    },
                    pricingPlans: new Dictionary<long, PricingPlans>
                    {
                        [initialServerState.DefaultWorkspace.Id] = PricingPlans.StarterMonthly // tasks are a starter feature
                    });

        protected override DatabaseState ArrangeDatabaseState(ServerState serverState)
            => new DatabaseState(
                user: serverState.User.ToSyncable(),
                preferences: serverState.Preferences.ToSyncable(),
                workspaces: serverState.Workspaces.ToSyncable(),
                projects: serverState.Projects.ToSyncable(),
                tasks: serverState.Tasks.ToSyncable(),
                tags: serverState.Tags.ToSyncable());

        protected override async Task Act(ISyncManager syncManager, AppServices appServices)
        {
            await syncManager.PullTimeEntries();
        }

        protected override void AssertFinalState(AppServices services, ServerState finalServerState, DatabaseState finalDatabaseState)
        {
            containsNoPlaceholdersFor(finalDatabaseState.Workspaces);
            containsNoPlaceholdersFor(finalDatabaseState.Projects);
            containsNoPlaceholdersFor(finalDatabaseState.Tasks);
            containsNoPlaceholdersFor(finalDatabaseState.Tags);

            finalDatabaseState.TimeEntries
                .Should()
                .HaveCount(3)
                .And.Contain(te => te.Description == "A" && te.ProjectId == finalDatabaseState.Projects.Single().Id)
                .And.Contain(te => te.Description == "B" && te.TaskId == finalDatabaseState.Tasks.Single().Id)
                .And.Contain(te => te.Description == "C" && te.TagIds.Single() == finalDatabaseState.Tags.Single().Id);
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
