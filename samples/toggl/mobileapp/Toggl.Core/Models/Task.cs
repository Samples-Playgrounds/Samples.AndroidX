using System;
using Toggl.Shared;
using Toggl.Storage;

namespace Toggl.Core.Models
{
    internal sealed partial class Task
    {
        internal sealed class Builder
        {
            private const string errorMessage = "You need to set the {0} before building a task.";

            public long Id { get; }
            public long? WorkspaceId { get; private set; }
            public string Name { get; private set; }
            public SyncStatus? SyncStatus { get; private set; }

            public static Builder Create(long id) => new Builder(id);

            private Builder(long id)
            {
                Id = id;
            }

            public Task Build()
            {
                ensureValidity();

                return new Task(this);
            }

            public Builder SetName(string name)
            {
                Name = name;
                return this;
            }

            public Builder SetWorkspaceId(long id)
            {
                WorkspaceId = id;
                return this;
            }

            public Builder SetSyncStatus(SyncStatus syncStatus)
            {
                SyncStatus = syncStatus;
                return this;
            }

            private void ensureValidity()
            {
                if (!WorkspaceId.HasValue)
                    throw new InvalidOperationException(string.Format(errorMessage, nameof(WorkspaceId)));

                if (string.IsNullOrEmpty(Name))
                    throw new InvalidOperationException(string.Format(errorMessage, nameof(Name)));

                if (!SyncStatus.HasValue)
                    throw new InvalidOperationException(string.Format(errorMessage, nameof(SyncStatus)));
            }
        }

        internal static Task CreatePlaceholder(long taskId, long workspaceId)
        {
            return Builder.Create(taskId)
                .SetWorkspaceId(workspaceId)
                .SetName(Resources.TaskPlaceholder)
                .SetSyncStatus(SyncStatus.RefetchingNeeded)
                .Build();
        }

        private Task(Builder builder)
        {
            Id = builder.Id;
            Name = builder.Name;
            SyncStatus = builder.SyncStatus.Value;
        }
    }
}
