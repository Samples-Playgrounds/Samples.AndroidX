using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync.ConflictResolution;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.DataSources
{
    internal sealed class UserDataSource : SingletonDataSource<IThreadSafeUser, IDatabaseUser>
    {
        public UserDataSource(ISingleObjectStorage<IDatabaseUser> storage)
            : base(storage, null)
        {
        }

        protected override IThreadSafeUser Convert(IDatabaseUser entity)
            => User.From(entity);

        protected override ConflictResolutionMode ResolveConflicts(IDatabaseUser first, IDatabaseUser second)
            => Resolver.ForUser.Resolve(first, second);
    }
}
