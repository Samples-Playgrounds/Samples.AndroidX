using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    public sealed class ObserveTimeEntriesVisibleToTheUserInteractor : IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>>
    {
        private readonly IObservableDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> dataSource;

        public ObserveTimeEntriesVisibleToTheUserInteractor(IObservableDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> dataSource)
        {
            this.dataSource = dataSource;
        }

        public IObservable<IEnumerable<IThreadSafeTimeEntry>> Execute()
        {
            return dataSource.ItemsChanged
                .StartWith(Unit.Default)
                .SelectMany(_ => new GetAllTimeEntriesVisibleToTheUserInteractor(dataSource).Execute());
        }
    }
}
