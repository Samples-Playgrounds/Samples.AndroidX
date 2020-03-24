using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Sync.States.Pull
{
    public sealed class DeleteInaccessibleRunningTimeEntryState : ISyncState<IFetchObservables>
    {
        private readonly IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> dataSource;

        public StateResult<IFetchObservables> Done { get; } = new StateResult<IFetchObservables>();

        public DeleteInaccessibleRunningTimeEntryState(IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            this.dataSource = dataSource;
        }

        public IObservable<ITransition> Start(IFetchObservables fetch)
            => dataSource
                .GetAll(inaccessibleSyncedRunningTimeEntry, includeInaccessibleEntities: true)
                .Flatten()
                .SelectMany(deleteIfNeeded)
                .ToList()
                .Select(_ => Done.Transition(fetch));

        private bool inaccessibleSyncedRunningTimeEntry(IDatabaseTimeEntry timeEntry)
            => timeEntry.IsInaccessible &&
               timeEntry.SyncStatus == SyncStatus.InSync &&
               timeEntry.Duration == null;

        private IObservable<Unit> deleteIfNeeded(IThreadSafeTimeEntry timeEntry)
            => timeEntry == null
                ? Observable.Return(Unit.Default)
                : dataSource.Delete(timeEntry.Id);
    }
}
