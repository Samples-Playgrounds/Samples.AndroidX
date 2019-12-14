using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Toggl.Shared.Models;
using Toggl.Storage.Realm.Models;

namespace Toggl.Storage.Realm
{
    internal sealed class Repository<TModel> : BaseStorage<TModel>, IRepository<TModel>
    {
        public Repository(IRealmAdapter<TModel> adapter)
            : base(adapter) { }

        public IObservable<TModel> GetById(long id)
            => CreateObservable(() => Adapter.Get(id));

        public IObservable<IEnumerable<TModel>> GetByIds(long[] ids)
            => CreateObservable(() => Adapter.Get(ids));

        public IObservable<TModel> ChangeId(long currentId, long newId)
            => CreateObservable(() => Adapter.ChangeId(currentId, newId));

        public static Repository<TModel> For<TRealmEntity>(
            Func<Realms.Realm> getRealmInstance, Func<TModel, Realms.Realm, TRealmEntity> convertToRealm)
            where TRealmEntity : RealmObject, IIdentifiable, IModifiableId, TModel, IUpdatesFrom<TModel>
            => For(getRealmInstance, convertToRealm, matchById<TRealmEntity>, matchByIds<TRealmEntity>, getId<TRealmEntity>);

        public static Repository<TModel> For<TRealmEntity>(
            Func<Realms.Realm> getRealmInstance,
            Func<TModel, Realms.Realm, TRealmEntity> convertToRealm,
            Func<long, Expression<Func<TRealmEntity, bool>>> matchById,
            Func<long[], Expression<Func<TRealmEntity, bool>>> matchByIds,
            Func<TRealmEntity, long> getId)
            where TRealmEntity : RealmObject, TModel, IUpdatesFrom<TModel>
            => new Repository<TModel>(new RealmAdapter<TRealmEntity, TModel>(getRealmInstance, convertToRealm, matchById, matchByIds, getId));

        private static Expression<Func<TRealmEntity, bool>> matchById<TRealmEntity>(long id)
            where TRealmEntity : RealmObject, IIdentifiable, IModifiableId, TModel, IUpdatesFrom<TModel>
            => x => x.Id == id || x.OriginalId == id;

        private static Expression<Func<TRealmEntity, bool>> matchByIds<TRealmEntity>(long[] ids)
            where TRealmEntity : RealmObject, IIdentifiable, IModifiableId, TModel, IUpdatesFrom<TModel>
        {
            var nullableIds = ids.Select(id => (long?)id).ToArray();

            var entityParam = Expression.Parameter(typeof(TRealmEntity), "entity");
            var idProperty = Expression.Property(entityParam, typeof(TRealmEntity), "Id");
            var originalIdProperty = Expression.Property(entityParam, typeof(TRealmEntity), "OriginalId");

            var containsIdExpr = contains(idProperty, ids);
            var containsOriginalIdExpr = contains(originalIdProperty, nullableIds);

            var matchExpression = Expression.OrElse(containsIdExpr, containsOriginalIdExpr);

            return Expression.Lambda<Func<TRealmEntity, bool>>(matchExpression, new[] { entityParam });
        }

        private static long getId<TRealmEntity>(TRealmEntity entity)
            where TRealmEntity : RealmObject, IIdentifiable, TModel
            => entity.Id;

        private static Expression contains<T>(Expression nullableProperty, T[] ids)
        {
            var equalsList = ids.Select(id => Expression.Equal(nullableProperty, Expression.Convert(Expression.Constant(id), typeof(T))));
            return equalsList.Aggregate(Expression.Constant(false) as Expression, (partial, expr) => Expression.OrElse(expr, partial));
        }
    }
}
