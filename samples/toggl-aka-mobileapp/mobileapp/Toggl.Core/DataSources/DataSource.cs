using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Extensions;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Models;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.DataSources
{
    public abstract class DataSource<TThreadsafe, TDatabase>
        : BaseDataSource<TThreadsafe, TDatabase>,
          IDataSource<TThreadsafe, TDatabase>
        where TDatabase : IDatabaseModel
        where TThreadsafe : TDatabase, IThreadSafeModel, IIdentifiable
    {
        private readonly IRepository<TDatabase> repository;

        protected DataSource(IRepository<TDatabase> repository)
            : base(repository)
        {
            Ensure.Argument.IsNotNull(repository, nameof(repository));

            this.repository = repository;
        }

        public IObservable<TThreadsafe> GetById(long id)
            => repository.GetById(id).Select(Convert);

        public IObservable<IEnumerable<TThreadsafe>> GetByIds(long[] ids)
            => repository.GetByIds(ids).Select(entities => entities.Select(Convert));

        public virtual IObservable<TThreadsafe> ChangeId(long currentId, long newId)
            => repository.ChangeId(currentId, newId).Select(Convert);

        public virtual IObservable<IEnumerable<TThreadsafe>> GetAll(bool includeInaccessibleEntities = false)
        {
            var repositoryEntities = includeInaccessibleEntities
                ? repository.GetAll()
                : repository.GetAll(isAccessible);

            return repositoryEntities.Select(entities => entities.Select(Convert));
        }

        public virtual IObservable<IEnumerable<TThreadsafe>> GetAll(Func<TDatabase, bool> predicate, bool includeInaccessibleEntities = false)
        {
            var repositoryEntities = includeInaccessibleEntities
                ? repository.GetAll(predicate)
                : repository.GetAll(entity => predicate(entity) && isAccessible(entity));

            return repositoryEntities.Select(entities => entities.Select(Convert));
        }

        public virtual IObservable<IEnumerable<IConflictResolutionResult<TThreadsafe>>> DeleteAll(IEnumerable<TThreadsafe> entities)
            => repository.BatchUpdate(convertEntitiesForBatchUpdate(entities), safeAlwaysDelete)
                         .ToThreadSafeResult(Convert);

        public virtual IObservable<Unit> Delete(long id)
            => repository.Delete(id);

        public virtual IObservable<IEnumerable<IConflictResolutionResult<TThreadsafe>>> BatchUpdate(IEnumerable<TThreadsafe> entities)
            => repository.BatchUpdate(
                    convertEntitiesForBatchUpdate(entities),
                    ResolveConflicts,
                    RivalsResolver)
                .ToThreadSafeResult(Convert);

        private IEnumerable<(long, TDatabase)> convertEntitiesForBatchUpdate(
            IEnumerable<TThreadsafe> entities)
            => entities.Select(entity => (entity.Id, (TDatabase)entity));

        private static ConflictResolutionMode safeAlwaysDelete(TDatabase old, TDatabase now)
            => old == null ? ConflictResolutionMode.Ignore : ConflictResolutionMode.Delete;

        private bool isAccessible(TDatabase entity)
        {
            if (entity is IPotentiallyInaccessible potentiallyInaccessible)
                return !potentiallyInaccessible.IsInaccessible;

            return true;
        }
    }
}
