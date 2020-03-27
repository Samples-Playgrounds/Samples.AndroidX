using System;
using System.Linq;
using System.Linq.Expressions;

namespace Toggl.Storage
{
    public interface IRivalsResolver<TModel>
    {
        bool CanHaveRival(TModel entity);
        Expression<Func<TModel, bool>> AreRivals(TModel entity);
        (TModel FixedEntity, TModel FixedRival) FixRivals<TRealmObject>(TModel entity, TModel rival, IQueryable<TRealmObject> allEntities)
            where TRealmObject : TModel;
    }
}
