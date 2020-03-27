using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync.ConflictResolution;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.DataSources
{
    internal sealed class ClientsDataSource
        : DataSource<IThreadSafeClient, IDatabaseClient>
    {
        public ClientsDataSource(IRepository<IDatabaseClient> repository)
            : base(repository)
        {
        }

        protected override IThreadSafeClient Convert(IDatabaseClient entity)
            => Client.From(entity);

        protected override ConflictResolutionMode ResolveConflicts(IDatabaseClient first, IDatabaseClient second)
            => Resolver.ForClients.Resolve(first, second);
    }
}
