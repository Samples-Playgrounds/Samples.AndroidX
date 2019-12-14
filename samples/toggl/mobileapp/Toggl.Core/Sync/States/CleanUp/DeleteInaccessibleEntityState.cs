using System;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Sync.States.CleanUp
{
    public abstract class DeleteInaccessibleEntityState<TInterface, TDatabaseInterface>
        : ISyncState
        where TInterface : class, TDatabaseInterface, IThreadSafeModel
        where TDatabaseInterface : IDatabaseSyncable, IPotentiallyInaccessible
    {
        private readonly IDataSource<TInterface, TDatabaseInterface> dataSource;

        public StateResult Done { get; } = new StateResult();

        public DeleteInaccessibleEntityState(
            IDataSource<TInterface, TDatabaseInterface> dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            this.dataSource = dataSource;
        }

        public IObservable<ITransition> Start()
            => dataSource.GetAll(candidateForDeletion, includeInaccessibleEntities: true)
                .Flatten()
                .WhereAsync(SuitableForDeletion)
                .ToList()
                .SelectMany(dataSource.DeleteAll)
                .SelectValue(Done.Transition());

        protected abstract IObservable<bool> SuitableForDeletion(TInterface entity);

        private bool candidateForDeletion(TDatabaseInterface entity)
            => entity.IsInaccessible && entity.SyncStatus == SyncStatus.InSync;
    }
}
