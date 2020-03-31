using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared.Models;
using Toggl.Storage;

namespace Toggl.Core.Sync.States.Push
{
    internal sealed class DeleteLocalEntityState<TDatabaseModel, TThreadsafeModel> : ISyncState<TThreadsafeModel>
        where TDatabaseModel : class, IDatabaseSyncable
        where TThreadsafeModel : TDatabaseModel, IThreadSafeModel, IIdentifiable
    {
        private IDataSource<TThreadsafeModel, TDatabaseModel> dataSource { get; }

        public StateResult Done { get; } = new StateResult();

        public StateResult DeletingFailed { get; } = new StateResult();

        public DeleteLocalEntityState(IDataSource<TThreadsafeModel, TDatabaseModel> dataSource)
        {
            this.dataSource = dataSource;
        }

        public IObservable<ITransition> Start(TThreadsafeModel entity)
            => delete(entity)
                .Select(_ => Done.Transition())
                .Catch((Exception e) => Observable.Return(DeletingFailed.Transition()));

        private IObservable<Unit> delete(TThreadsafeModel entity)
            => entity == null
                ? Observable.Throw<Unit>(new ArgumentNullException(nameof(entity)))
                : dataSource.Delete(entity.Id);
    }
}
