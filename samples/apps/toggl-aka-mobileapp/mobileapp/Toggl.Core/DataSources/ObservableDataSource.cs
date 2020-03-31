using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Storage;

namespace Toggl.Core.DataSources
{
    public abstract class ObservableDataSource<TThreadsafe, TDatabase>
        : DataSource<TThreadsafe, TDatabase>, IObservableDataSource<TThreadsafe, TDatabase>
        where TDatabase : IDatabaseSyncable
        where TThreadsafe : IThreadSafeModel, IIdentifiable, TDatabase
    {
        private readonly Subject<Unit> itemsChangedSubject = new Subject<Unit>();

        public IObservable<Unit> ItemsChanged { get; }

        protected ObservableDataSource(IRepository<TDatabase> repository)
            : base(repository)
        {
            ItemsChanged = itemsChangedSubject.AsObservable();
        }

        public override IObservable<TThreadsafe> Create(TThreadsafe entity)
            => base.Create(entity).Do(ReportChange);

        public override IObservable<TThreadsafe> Update(TThreadsafe entity)
            => base.Update(entity).Do(ReportChange);

        public override IObservable<TThreadsafe> ChangeId(long currentId, long newId)
            => base.ChangeId(currentId, newId).Do(ReportChange);

        public override IObservable<Unit> Delete(long id)
            => base.Delete(id).Do(ReportChange);

        public override IObservable<IEnumerable<IConflictResolutionResult<TThreadsafe>>> OverwriteIfOriginalDidNotChange(TThreadsafe original, TThreadsafe entity)
            => base.OverwriteIfOriginalDidNotChange(original, entity).Do(ReportChange);

        public override IObservable<IEnumerable<IConflictResolutionResult<TThreadsafe>>> BatchUpdate(IEnumerable<TThreadsafe> entities)
            => base.BatchUpdate(entities).Do(ReportChange);

        public override IObservable<IEnumerable<IConflictResolutionResult<TThreadsafe>>> DeleteAll(IEnumerable<TThreadsafe> entities)
            => base.DeleteAll(entities).Do(ReportChange);

        protected void ReportChange()
        {
            itemsChangedSubject.OnNext(Unit.Default);
        }
    }
}
