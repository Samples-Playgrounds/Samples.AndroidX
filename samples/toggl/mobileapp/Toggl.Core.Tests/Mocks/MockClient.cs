using System;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Tests.Mocks
{
    public sealed class MockClient : IThreadSafeClient
    {
        IDatabaseWorkspace IDatabaseClient.Workspace => Workspace;

        public long WorkspaceId { get; set; }

        public string Name { get; set; }

        public DateTimeOffset At { get; set; }

        public DateTimeOffset? ServerDeletedAt { get; set; }

        public long Id { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string LastSyncErrorMessage { get; set; }

        public bool IsDeleted { get; set; }

        public IThreadSafeWorkspace Workspace { get; set; }

        public bool IsInaccessible => Workspace.IsInaccessible;

        public MockClient() { }

        public MockClient(
            long id,
            IThreadSafeWorkspace workspace,
            SyncStatus syncStatus = SyncStatus.InSync
        ) : this()
        {
            Id = id;
            Workspace = workspace;
            WorkspaceId = workspace.Id;
            SyncStatus = syncStatus;
        }
    }
}
