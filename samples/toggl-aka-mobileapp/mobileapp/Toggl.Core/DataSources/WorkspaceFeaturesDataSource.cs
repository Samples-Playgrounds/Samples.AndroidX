using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync.ConflictResolution;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.DataSources
{
    internal sealed class WorkspaceFeaturesDataSource : DataSource<IThreadSafeWorkspaceFeatureCollection, IDatabaseWorkspaceFeatureCollection>
    {
        public WorkspaceFeaturesDataSource(IRepository<IDatabaseWorkspaceFeatureCollection> repository)
            : base(repository)
        {
        }

        protected override IThreadSafeWorkspaceFeatureCollection Convert(IDatabaseWorkspaceFeatureCollection entity)
            => WorkspaceFeatureCollection.From(entity);

        protected override ConflictResolutionMode ResolveConflicts(IDatabaseWorkspaceFeatureCollection first,
            IDatabaseWorkspaceFeatureCollection second)
            => Resolver.ForWorkspaceFeatures.Resolve(first, second);
    }
}
