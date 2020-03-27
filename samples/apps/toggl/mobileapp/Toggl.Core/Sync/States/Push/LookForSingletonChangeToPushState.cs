using System;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync.States.Push.Interfaces;
using Toggl.Shared;
using Toggl.Storage;

namespace Toggl.Core.Sync.States.Push
{
    internal sealed class LookForSingletonChangeToPushState<T> : ILookForChangeToPushState<T>
        where T : class, IThreadSafeModel, IDatabaseSyncable
    {
        private readonly ISingletonDataSource<T> dataSource;

        public StateResult<T> ChangeFound { get; } = new StateResult<T>();

        public StateResult NoMoreChanges { get; } = new StateResult();

        public LookForSingletonChangeToPushState(ISingletonDataSource<T> dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;
        }

        public IObservable<ITransition> Start() =>
            dataSource
                .Get()
                .Where(entity => entity.SyncStatus == SyncStatus.SyncNeeded)
                .SingleOrDefaultAsync()
                .Select(entity =>
                    entity != null
                        ? (ITransition)ChangeFound.Transition(entity)
                        : NoMoreChanges.Transition());
    }
}
