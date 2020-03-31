using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync.ConflictResolution;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.DataSources
{
    internal sealed class TagsDataSource : DataSource<IThreadSafeTag, IDatabaseTag>
    {
        public TagsDataSource(IRepository<IDatabaseTag> repository)
            : base(repository)
        {
        }

        protected override IThreadSafeTag Convert(IDatabaseTag entity)
            => Tag.From(entity);

        protected override ConflictResolutionMode ResolveConflicts(IDatabaseTag first, IDatabaseTag second)
            => Resolver.ForTags.Resolve(first, second);
    }
}
