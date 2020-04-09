using System;
using System.Reactive;
using Toggl.Core.DataSources;
using Toggl.Shared;

namespace Toggl.Core.Interactors
{
    public class ObserveTimeEntriesChangesInteractor : IInteractor<IObservable<Unit>>
    {
        private readonly ITogglDataSource dataSource;

        public ObserveTimeEntriesChangesInteractor(ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;
        }

        public IObservable<Unit> Execute()
            => dataSource.TimeEntries.ItemsChanged;
    }
}
