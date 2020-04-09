using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Shared.Models;
using Toggl.Storage.Realm;
using Toggl.Storage.Realm.Models;

namespace Toggl.Storage.Tests.Realm
{
    public class GenericTestAdapter<T> : IRealmAdapter<T>
        where T : class, IIdentifiable
    {
        private readonly List<T> list = new List<T>();
        private readonly Func<long, Predicate<T>> matchById;
        private readonly Func<long[], Predicate<T>> matchByIds;

        public GenericTestAdapter()
            : this(id => e => e.Id == id, ids => e => ids.Contains(e.Id))
        {
        }

        public GenericTestAdapter(Func<long, Predicate<T>> matchById, Func<long[], Predicate<T>> matchByIds)
        {
            this.matchById = matchById;
            this.matchByIds = matchByIds;
        }

        public T Get(long id)
            => list.Single(entity => matchById(id)(entity));

        public IEnumerable<T> Get(long[] ids)
            => list.Where(entity => matchByIds(ids)(entity));

        public T ChangeId(long currentId, long newId)
        {
            var entity = Get(currentId);

            if (entity is IModifiableId modifiableEntity)
            {
                modifiableEntity.ChangeId(newId);
                return entity;
            }

            throw new InvalidOperationException($"Cannot change ID of entity of type {typeof(T)} which does not implement {nameof(IModifiableId)}");
        }

        public T Create(T entity)
        {
            if (list.Find(matchById(entity.Id)) != null)
                throw new InvalidOperationException();

            list.Add(entity);

            return entity;
        }

        public T Update(long id, T entity)
        {
            var index = list.FindIndex(matchById(id));

            if (index == -1)
                throw new InvalidOperationException();

            list[index] = entity;

            return entity;
        }

        public IQueryable<T> GetAll()
            => list.AsQueryable();

        public void Delete(long id)
        {
            var entity = Get(id);
            var worked = list.Remove(entity);
            if (worked) return;

            throw new InvalidOperationException();
        }

        public IEnumerable<IConflictResolutionResult<T>> BatchUpdate(
            IEnumerable<(long Id, T Entity)> entities,
            Func<T, T, ConflictResolutionMode> conflictResolution,
            IRivalsResolver<T> resolver)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class TestAdapter : GenericTestAdapter<TestModel>
    {
        public TestAdapter()
            : base()
        {
        }

        public TestAdapter(Func<long, Predicate<TestModel>> matchById, Func<long[], Predicate<TestModel>> matchByIds)
            : base(matchById, matchByIds)
        {
        }
    }
}
