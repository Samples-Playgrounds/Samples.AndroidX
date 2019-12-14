using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync.States.Push.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Models;
using Toggl.Storage;

namespace Toggl.Core.Sync.States.Push
{
    internal sealed class LookForChangeToPushState<TDatabaseModel, TThreadsafeModel> : ILookForChangeToPushState<TThreadsafeModel>
        where TDatabaseModel : IDatabaseSyncable
        where TThreadsafeModel : class, TDatabaseModel, ILastChangedDatable, IThreadSafeModel
    {
        private readonly IDataSource<TThreadsafeModel, TDatabaseModel> dataSource;

        public StateResult<TThreadsafeModel> ChangeFound { get; } = new StateResult<TThreadsafeModel>();

        public StateResult NoMoreChanges { get; } = new StateResult();

        public LookForChangeToPushState(IDataSource<TThreadsafeModel, TDatabaseModel> dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;
        }

        public IObservable<ITransition> Start() =>
            getOldestUnsynced()
                .SingleAsync()
                .Select(entity =>
                    entity != null
                        ? (ITransition)ChangeFound.Transition(entity)
                        : NoMoreChanges.Transition());

        private IObservable<TThreadsafeModel> getOldestUnsynced()
            => dataSource
                .GetAll(entity => entity.SyncStatus == SyncStatus.SyncNeeded)
                .SingleAsync()
                .Select(entities => entities
                    .OrderBy(entity => entity.At)
                    .FirstOrDefault());
    }
}
