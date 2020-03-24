using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync.ConflictResolution;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.DataSources
{
    internal sealed class TasksDataSource : DataSource<IThreadSafeTask, IDatabaseTask>
    {
        public TasksDataSource(IRepository<IDatabaseTask> repository)
            : base(repository)
        {
        }

        protected override IThreadSafeTask Convert(IDatabaseTask entity)
            => Task.From(entity);

        protected override ConflictResolutionMode ResolveConflicts(IDatabaseTask first, IDatabaseTask second)
            => Resolver.ForTasks.Resolve(first, second);
    }
}
