using Realms;
using System;
using Toggl.Storage.Models;

namespace Toggl.Storage.Realm
{
    internal partial class RealmTag : RealmObject, IDatabaseTag
    {
        public string Name { get; set; }

        public DateTimeOffset At { get; set; }

        public RealmWorkspace RealmWorkspace { get; set; }

        public long WorkspaceId => RealmWorkspace?.Id ?? 0;

        public IDatabaseWorkspace Workspace => RealmWorkspace;

        public DateTimeOffset? ServerDeletedAt { get; set; }

        public bool IsInaccessible => Workspace.IsInaccessible;
    }
}
