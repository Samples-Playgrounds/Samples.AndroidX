using System.Collections.Generic;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared.Models;
using Toggl.Storage.Models;

namespace Toggl.Core.Tests.Mocks
{
    public sealed class MockWorkspaceFeatureCollection : IThreadSafeWorkspaceFeatureCollection
    {
        public long Id => WorkspaceId;

        IDatabaseWorkspace IDatabaseWorkspaceFeatureCollection.Workspace => Workspace;

        IEnumerable<IDatabaseWorkspaceFeature> IDatabaseWorkspaceFeatureCollection.DatabaseFeatures => DatabaseFeatures;

        public long WorkspaceId { get; set; }

        public IEnumerable<IWorkspaceFeature> Features { get; set; }

        public IThreadSafeWorkspace Workspace { get; set; }

        public IEnumerable<IThreadSafeWorkspaceFeature> DatabaseFeatures { get; set; }
    }
}
