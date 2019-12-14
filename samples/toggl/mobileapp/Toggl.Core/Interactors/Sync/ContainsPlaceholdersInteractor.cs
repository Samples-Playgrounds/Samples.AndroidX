using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage;

namespace Toggl.Core.Interactors
{
    public sealed class ContainsPlaceholdersInteractor : IInteractor<IObservable<bool>>
    {
        private readonly ITogglDataSource dataSource;

        public ContainsPlaceholdersInteractor(ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;
        }

        public IObservable<bool> Execute()
            => Observable.Merge(
                    containsPlaceholders(dataSource.Workspaces),
                    containsPlaceholders(dataSource.Projects),
                    containsPlaceholders(dataSource.Tasks),
                    containsPlaceholders(dataSource.Tags))
                .Where(hasPlaceholders => hasPlaceholders)
                .Any();

        private IObservable<bool> containsPlaceholders<TThreadSafe, TDatabase>(
            IDataSource<TThreadSafe, TDatabase> entityDataSource)
            where TThreadSafe : IThreadSafeModel, TDatabase
            where TDatabase : IDatabaseSyncable
            => entityDataSource
                .GetAll(entity => entity.SyncStatus == SyncStatus.RefetchingNeeded)
                .Select(placeholders => placeholders.Any());
    }
}
