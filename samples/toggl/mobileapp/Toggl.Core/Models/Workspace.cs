using System;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage;

namespace Toggl.Core.Models
{
    internal partial class Workspace
    {
        internal sealed class Builder
        {
            public static Builder Create(long id) => new Builder(id);

            public long Id { get; }

            public string Name { get; private set; }

            public SyncStatus SyncStatus { get; private set; }

            public DateTimeOffset At { get; private set; }

            private Builder(long id)
            {
                Id = id;
            }

            public Workspace Build()
            {
                return new Workspace(this);
            }

            public Builder SetName(string name)
            {
                Name = name;
                return this;
            }

            public Builder SetSyncStatus(SyncStatus syncStatus)
            {
                SyncStatus = syncStatus;
                return this;
            }

            public Builder SetAt(DateTimeOffset at)
            {
                At = at;
                return this;
            }
        }

        internal static Workspace CreatePlaceholder(long id)
        {
            return Builder.Create(id)
                .SetName(Resources.WorkspacePlaceholder)
                .SetSyncStatus(SyncStatus.RefetchingNeeded)
                .Build();
        }

        private Workspace(Builder builder)
        {
            Id = builder.Id;
            Name = builder.Name;
            SyncStatus = builder.SyncStatus;
            At = builder.At;
        }

        internal Workspace(IThreadSafeWorkspace workspace, bool isInaccessible)
            : this(workspace)
        {
            IsInaccessible = isInaccessible;
        }
    }

    public static class ThreadSafeWorkspaceExtensions
    {
        public static IThreadSafeWorkspace AsInaccessible(this IThreadSafeWorkspace workspace)
            => new Workspace(workspace, isInaccessible: true);
    }
}
