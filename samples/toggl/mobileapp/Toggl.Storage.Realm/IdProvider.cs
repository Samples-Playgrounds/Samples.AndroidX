using Realms;
using System;
using System.Linq;
using Toggl.Shared;

namespace Toggl.Storage.Realm
{
    public sealed class RealmIdProvider : RealmObject
    {
        [PrimaryKey]
        public int Key { get; set; }

        public long Id { get; set; }
    }

    public sealed class IdProvider : IIdProvider
    {
        private readonly Func<Realms.Realm> getRealmInstance;

        public IdProvider(Func<Realms.Realm> getRealmInstance)
        {
            Ensure.Argument.IsNotNull(getRealmInstance, nameof(getRealmInstance));

            this.getRealmInstance = getRealmInstance;
        }

        public long GetNextIdentifier()
        {
            var nextIdentifier = -1L;
            var realm = getRealmInstance();
            using (var transaction = realm.BeginWrite())
            {
                var entity = realm.All<RealmIdProvider>().SingleOrDefault();
                if (entity == null)
                {
                    entity = new RealmIdProvider { Id = -2 };
                    realm.Add(entity);
                }
                else
                {
                    nextIdentifier = entity.Id;
                    entity.Id = nextIdentifier - 1;
                }

                transaction.Commit();
            }

            return nextIdentifier;
        }
    }
}
