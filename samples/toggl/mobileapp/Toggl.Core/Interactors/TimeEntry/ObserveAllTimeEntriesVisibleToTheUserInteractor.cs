using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    internal sealed class ObserveAllTimeEntriesVisibleToTheUserInteractor : IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>>
    {
        private readonly IObservableDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> timeEntries;
        private readonly IObservableDataSource<IThreadSafeWorkspace, IDatabaseWorkspace> workspaces;

        public ObserveAllTimeEntriesVisibleToTheUserInteractor(
            IObservableDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> timeEntries,
            IObservableDataSource<IThreadSafeWorkspace, IDatabaseWorkspace> workspaces)
        {
            Ensure.Argument.IsNotNull(timeEntries, nameof(timeEntries));
            Ensure.Argument.IsNotNull(workspaces, nameof(workspaces));

            this.timeEntries = timeEntries;
            this.workspaces = workspaces;
        }

        public IObservable<IEnumerable<IThreadSafeTimeEntry>> Execute()
            => Observable.Merge(
                    timeEntries.ItemsChanged,
                    workspaces.ItemsChanged)
                .StartWith(Unit.Default)
                .SelectMany(_ => getTimeEntries())
                .DistinctUntilChanged();

        private IObservable<IEnumerable<IThreadSafeTimeEntry>> getTimeEntries()
            => new GetAllTimeEntriesVisibleToTheUserInteractor(timeEntries)
                .Execute()
                .SingleAsync();
    }
}
