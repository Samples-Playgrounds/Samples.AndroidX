using Realms;
using System;
using Toggl.Storage.Queries;
using Toggl.Storage.Realm.Queries;

namespace Toggl.Storage.Tests.Realm.Queries
{
    public abstract class QueryTests : IDisposable
    {
        protected IQueryFactory QueryFactory { get; }
        protected Func<Realms.Realm> RealmProvider { get; }

        protected QueryTests()
        {
            var databaseId = $"{Guid.NewGuid().ToString()}.realm";
            var configuration = new InMemoryConfiguration(databaseId);
            RealmProvider = () => Realms.Realm.GetInstance(configuration);
            QueryFactory = new RealmQueryFactory(RealmProvider);
        }

        public virtual void Dispose()
        {
        }
    }
}
