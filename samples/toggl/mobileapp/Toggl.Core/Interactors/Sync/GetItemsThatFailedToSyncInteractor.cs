using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage;

namespace Toggl.Core.Interactors
{
    public class GetItemsThatFailedToSyncInteractor : IInteractor<IObservable<IEnumerable<SyncFailureItem>>>
    {
        private readonly ITogglDataSource dataSource;

        public GetItemsThatFailedToSyncInteractor(ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;
        }

        public IObservable<IEnumerable<SyncFailureItem>> Execute()
        {
            return Observable
                .Concat(
                    getUnsyncedItems(dataSource.Clients),
                    getUnsyncedItems(dataSource.Projects),
                    getUnsyncedItems(dataSource.Tags)
                )
                .ToList();
        }

        private IObservable<SyncFailureItem> getUnsyncedItems<TThreadsafe, TDatabase>(
            IDataSource<TThreadsafe, TDatabase> source)
            where TDatabase : IDatabaseSyncable
            where TThreadsafe : TDatabase, IThreadSafeModel
        {
            return source
                .GetAll(p => p.SyncStatus == SyncStatus.SyncFailed)
                .SelectMany(convertToSyncFailures);
        }

        private IEnumerable<SyncFailureItem> convertToSyncFailures<T>(IEnumerable<T> items)
            where T : IThreadSafeModel, IDatabaseSyncable
        {
            return items.Select(i => new SyncFailureItem(i));
        }
    }
}
