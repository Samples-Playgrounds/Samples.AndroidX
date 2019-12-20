using Realms;
using System.Collections.Generic;
using Toggl.Shared.Models;
using Toggl.Storage.Models;

namespace Toggl.Storage.Realm
{
    internal partial class RealmWorkspaceFeatureCollection : RealmObject, IDatabaseWorkspaceFeatureCollection
    {
        [Ignored]
        public RealmWorkspace RealmWorkspace
        {
            get => RealmWorkspaceInternal;
            set
            {
                WorkspaceId = value.Id;
                RealmWorkspaceInternal = value;
            }
        }

        public RealmWorkspace RealmWorkspaceInternal { get; set; }

        public long WorkspaceId { get; set; }

        public IDatabaseWorkspace Workspace => RealmWorkspace;

        public IList<RealmWorkspaceFeature> RealmWorkspaceFeatures { get; }

        public IEnumerable<IDatabaseWorkspaceFeature> DatabaseFeatures => RealmWorkspaceFeatures;

        public IEnumerable<IWorkspaceFeature> Features => RealmWorkspaceFeatures;
    }
}
