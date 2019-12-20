using System;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Shared;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Sync.States.CleanUp
{
    public class DeleteInaccessibleTimeEntriesState : ISyncState
    {
        private readonly ITimeEntriesSource dataSource;

        public StateResult Done { get; } = new StateResult();

        public DeleteInaccessibleTimeEntriesState(ITimeEntriesSource dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            this.dataSource = dataSource;
        }

        public IObservable<ITransition> Start()
            => dataSource
                .GetAll(suitableForDeletion, includeInaccessibleEntities: true)
                .SelectMany(dataSource.DeleteAll)
                .Select(_ => Done.Transition());

        private bool suitableForDeletion(IDatabaseTimeEntry timeEntry)
            => timeEntry.IsInaccessible && isSynced(timeEntry);

        private bool isSynced(IDatabaseTimeEntry timeEntry)
            => timeEntry.SyncStatus == SyncStatus.InSync;
    }
}
