using System;
using Toggl.Shared;
using Toggl.Storage.Queries;

namespace Toggl.Storage.Realm.Queries
{
    public sealed class RealmQueryFactory : IQueryFactory
    {
        private readonly Func<Realms.Realm> realmProvider;

        public RealmQueryFactory(Func<Realms.Realm> realmProvider)
        {
            Ensure.Argument.IsNotNull(realmProvider, nameof(realmProvider));

            this.realmProvider = realmProvider;
        }
    }
}
