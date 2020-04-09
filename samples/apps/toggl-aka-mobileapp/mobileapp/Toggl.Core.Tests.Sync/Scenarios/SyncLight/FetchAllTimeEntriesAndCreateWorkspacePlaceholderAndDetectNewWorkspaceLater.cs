using FluentAssertions;
using System;
using System.Collections.Generic;
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
    public sealed class FetchAllTimeEntriesAndCreateWorkspacePlaceholderAndDetectNewWorkspaceLater : ComplexSyncTest
    {
        protected override ServerState ArrangeServerState(ServerState initialServerState)
            => initialServerState
                .With(
                    workspaces: new[] { initialServerState.DefaultWorkspace, new MockWorkspace { Id = -1, Name = "Workspace" } },
                    clients: new[] { new MockClient { Id = -2, Name = "Client", WorkspaceId = -1 } },
                    projects: new[] { new MockProject { Id = -3, Name = "Project", Color = "#06AAF5", Active = true, ClientId = -2, WorkspaceId = -1 } },
                    tasks: new[] { new MockTask { Id = -4, Name = "Task", ProjectId = -3, WorkspaceId = -1, Active = true } },
                    tags: new[]
                    {
                        new MockTag { Id = -5, Name = "Tag A", WorkspaceId = -1 },
                        new MockTag { Id = -6, Name = "Tag B", WorkspaceId = -1 },
                        new MockTag { Id = -7, Name = "Tag C", WorkspaceId = -1 }
                    },
                    timeEntries: new[]
                    {
                        new MockTimeEntry
                        {
                            Id = -8,
                            Description = "Time Entry",
                            Start = DateTimeOffset.Now,
                            WorkspaceId = -1,
                            ProjectId = -3,
                            TaskId = -4,
                            TagIds = new long[] { -5, -6, -7 }
                        }
                    },
                    pricingPlans: new Dictionary<long, PricingPlans>
                    {
                        [-1] = PricingPlans.StarterAnnual // tasks are a starter feature
                    });

        protected override DatabaseState ArrangeDatabaseState(ServerState serverState)
            => new DatabaseState(
                user: serverState.User.ToSyncable(),
                preferences: serverState.Preferences.ToSyncable(),
                workspaces: new[] { serverState.DefaultWorkspace.ToSyncable() });

        protected override async Task Act(ISyncManager syncManager, AppServices appServices)
        {
            await syncManager.PullTimeEntries();
            await syncManager.ForceFullSync();
        }

        protected override void AssertFinalState(AppServices services, ServerState finalServerState, DatabaseState finalDatabaseState)
        {
            finalDatabaseState.Workspaces.Should().HaveCount(2);
            finalDatabaseState.Workspaces.ContainsNoPlaceholders();

            finalDatabaseState.Projects.Should().HaveCount(1);
            finalDatabaseState.Projects.ContainsNoPlaceholders();

            finalDatabaseState.Tasks.Should().HaveCount(1);
            finalDatabaseState.Tasks.ContainsNoPlaceholders();

            finalDatabaseState.Tags.Should().HaveCount(3);
            finalDatabaseState.Tags.ContainsNoPlaceholders();

            finalDatabaseState.TimeEntries
                .Should()
                .HaveCount(1)
                .And.Contain(te => te.Description == "Time Entry" && te.SyncStatus == SyncStatus.InSync);
        }
    }
}
