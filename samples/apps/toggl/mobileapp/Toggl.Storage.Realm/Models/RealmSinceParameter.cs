using Realms;
using System;
using Toggl.Storage.Models;

namespace Toggl.Storage.Realm.Models
{
    public sealed class RealmSinceParameter : RealmObject, IDatabaseSinceParameter, IUpdatesFrom<IDatabaseSinceParameter>
    {
        [PrimaryKey]
        public long Id { get; set; }

        public DateTimeOffset? Since { get; set; }

        public RealmSinceParameter() { }

        public RealmSinceParameter(IDatabaseSinceParameter entity)
        {
            SetPropertiesFrom(entity, null);
        }

        public void SetPropertiesFrom(IDatabaseSinceParameter entity, Realms.Realm realm)
        {
            Id = entity.Id;
            Since = entity.Since;
        }
    }
}
