using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync.ConflictResolution;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.DataSources
{
    internal sealed class WorkspacesDataSource : ObservableDataSource<IThreadSafeWorkspace, IDatabaseWorkspace>
    {
        public WorkspacesDataSource(IRepository<IDatabaseWorkspace> repository)
            : base(repository)
        {
        }

        protected override IThreadSafeWorkspace Convert(IDatabaseWorkspace entity)
            => Workspace.From(entity);

        protected override ConflictResolutionMode ResolveConflicts(IDatabaseWorkspace first, IDatabaseWorkspace second)
            => Resolver.ForWorkspaces.Resolve(first, second);
    }
}
