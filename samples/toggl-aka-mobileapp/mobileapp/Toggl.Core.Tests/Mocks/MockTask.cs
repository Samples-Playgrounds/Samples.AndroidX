using System;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Tests.Mocks
{
    public sealed class MockTask : IThreadSafeTask
    {
        IDatabaseUser IDatabaseTask.User => User;

        IDatabaseProject IDatabaseTask.Project => Project;

        IDatabaseWorkspace IDatabaseTask.Workspace => Workspace;

        public string Name { get; set; }

        public long ProjectId { get; set; }

        public long WorkspaceId { get; set; }

        public long? UserId { get; set; }

        public long EstimatedSeconds { get; set; }

        public bool Active { get; set; }

        public DateTimeOffset At { get; set; }

        public long TrackedSeconds { get; set; }

        public long Id { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string LastSyncErrorMessage { get; set; }

        public bool IsDeleted { get; set; }

        public IThreadSafeUser User { get; set; }

        public IThreadSafeProject Project { get; set; }

        public IThreadSafeWorkspace Workspace { get; set; }

        public bool IsInaccessible => Workspace.IsInaccessible;

        public MockTask() { }

        public MockTask(
            long id,
            IThreadSafeWorkspace workspace,
            IThreadSafeProject project,
            SyncStatus syncStatus = SyncStatus.InSync
        ) : this()
        {
            Id = id;
            Workspace = workspace;
            WorkspaceId = workspace.Id;
            Project = project;
            ProjectId = project.Id;
            SyncStatus = syncStatus;
        }
    }
}
