using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    internal sealed class GetAllTimeEntriesVisibleToTheUserInteractor : IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>>
    {
        private readonly IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> dataSource;

        public GetAllTimeEntriesVisibleToTheUserInteractor(IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;
        }

        public IObservable<IEnumerable<IThreadSafeTimeEntry>> Execute()
            => dataSource.GetAll(
                    te => !te.IsDeleted && (!te.IsInaccessible || te.Id < 0),
                    includeInaccessibleEntities: true)
                .Select(forceImmediateEnumeration);

        private IEnumerable<IThreadSafeTimeEntry> forceImmediateEnumeration(
            IEnumerable<IThreadSafeTimeEntry> timeEntries)
            => timeEntries.ToArray();

    }
}
