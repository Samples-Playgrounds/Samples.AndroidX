using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage;

namespace Toggl.Core.Sync.States.Pull
{
    internal sealed class PersistSingletonState<TInterface, TDatabaseInterface, TThreadsafeInterface>
        : IPersistState
        where TDatabaseInterface : TInterface, IDatabaseSyncable
        where TThreadsafeInterface : class, TDatabaseInterface, IThreadSafeModel
    {
        private readonly ISingletonDataSource<TThreadsafeInterface> dataSource;

        private readonly Func<TInterface, TThreadsafeInterface> convertToThreadsafeEntity;

        public StateResult<IFetchObservables> Done { get; } = new StateResult<IFetchObservables>();

        public PersistSingletonState(
            ISingletonDataSource<TThreadsafeInterface> dataSource,
            Func<TInterface, TThreadsafeInterface> convertToThreadsafeEntity)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(convertToThreadsafeEntity, nameof(convertToThreadsafeEntity));

            this.dataSource = dataSource;
            this.convertToThreadsafeEntity = convertToThreadsafeEntity;
        }

        public IObservable<ITransition> Start(IFetchObservables fetch)
            => fetch.GetSingle<TInterface>()
                .SingleAsync()
                .SelectMany(entity => entity == null
                    ? Observable.Return(Unit.Default)
                    : dataSource.UpdateWithConflictResolution(convertToThreadsafeEntity(entity)).Select(_ => Unit.Default))
                .Select(_ => Done.Transition(fetch));
    }
}
