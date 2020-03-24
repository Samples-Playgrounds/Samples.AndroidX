using Realms;
using System.Linq;

namespace Toggl.Storage.Realm
{
    public sealed class RealmConfigurator
    {
        public RealmConfiguration Configuration { get; }
            = new RealmConfiguration
            {
                SchemaVersion = 8,
                MigrationCallback = (migration, oldSchemaVersion) =>
                {
                    if (oldSchemaVersion < 3)
                    {
                        // nothing needs explicit updating when updating from schema 0 up to 3
                    }

                    if (oldSchemaVersion < 4)
                    {
                        var newTags = migration.NewRealm.All<RealmTag>();
                        var oldTags = migration.OldRealm.All("RealmTag");
                        for (var i = 0; i < newTags.Count(); i++)
                        {
                            var oldTag = oldTags.ElementAt(i);
                            var newTag = newTags.ElementAt(i);
                            newTag.ServerDeletedAt = oldTag.DeletedAt;
                        }
                    }

                    if (oldSchemaVersion < 6)
                    {
                        // nothing needs explicit updating when updating from schema 4 up to 6
                    }

                    if (oldSchemaVersion < 7)
                    {
                        // RealmWorkspace: IsGhost was renamed to IsInaccessible
                        // A migration is not required because the property was not used until now
                    }

                    if (oldSchemaVersion < 8)
                    {
                        // RealmUser: Added new property Timezone
                        // A migration is not required because it's acceptable for the timezone to be unspecified (null)
                    }
                }
            };
    }
}
