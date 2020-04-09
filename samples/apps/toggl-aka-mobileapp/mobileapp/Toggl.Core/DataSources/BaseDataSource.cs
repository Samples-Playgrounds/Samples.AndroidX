using System;
using System.Collections.Generic;
using System.Linq;
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
    public abstract class BaseDataSource<TThreadsafe, TDatabase> : IBaseDataSource<TThreadsafe>
        where TDatabase : IDatabaseModel
        where TThreadsafe : IThreadSafeModel, IIdentifiable, TDatabase
    {
        protected virtual IRivalsResolver<TDatabase> RivalsResolver { get; } = null;

        private readonly IBaseStorage<TDatabase> repository;

        protected BaseDataSource(IBaseStorage<TDatabase> repository)
        {
            Ensure.Argument.IsNotNull(repository, nameof(repository));

            this.repository = repository;
        }

        public virtual IObservable<TThreadsafe> Create(TThreadsafe entity)
            => repository.Create(entity).Select(Convert);

        public virtual IObservable<TThreadsafe> Update(TThreadsafe entity)
            => repository.Update(entity.Id, entity).Select(Convert);

        public virtual IObservable<IEnumerable<IConflictResolutionResult<TThreadsafe>>> OverwriteIfOriginalDidNotChange(TThreadsafe original, TThreadsafe entity)
            => repository.UpdateWithConflictResolution(original.Id, entity, ignoreIfChangedLocally(original), RivalsResolver)
                .ToThreadSafeResult(Convert);

        private Func<TDatabase, TDatabase, ConflictResolutionMode> ignoreIfChangedLocally(TThreadsafe localEntity)
            => (currentLocal, serverEntity) => localEntity.DiffersFrom(currentLocal)
                ? ConflictResolutionMode.Ignore
                : ConflictResolutionMode.Update;

        protected abstract TThreadsafe Convert(TDatabase entity);

        protected abstract ConflictResolutionMode ResolveConflicts(TDatabase first, TDatabase second);
    }
}
