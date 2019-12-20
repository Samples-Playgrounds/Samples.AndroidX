using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Extensions;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync;
using Toggl.Shared.Extensions;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    public class SoftDeleteMultipleTimeEntriesInteractor : IInteractor<IObservable<Unit>>
    {
        private readonly IInteractorFactory interactorFactory;
        private readonly IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> dataSource;
        private readonly ITimeService timeService;
        private readonly ISyncManager syncManager;
        private readonly long[] ids;

        public SoftDeleteMultipleTimeEntriesInteractor(
            IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> dataSource,
            ITimeService timeService,
            ISyncManager syncManager,
            IInteractorFactory interactorFactory,
            long[] ids)
        {
            this.interactorFactory = interactorFactory;
            this.dataSource = dataSource;
            this.timeService = timeService;
            this.syncManager = syncManager;
            this.ids = ids;
        }

        public IObservable<Unit> Execute()
            => interactorFactory.GetMultipleTimeEntriesById(ids)
                .Execute()
                .Select(markAsDeleted)
                .SelectMany(dataSource.BatchUpdate)
                .SingleAsync()
                .Do(syncManager.InitiatePushSync)
                .SelectUnit();

        private IEnumerable<IThreadSafeTimeEntry> markAsDeleted(IEnumerable<IThreadSafeTimeEntry> timeEntries)
            => timeEntries.Select(TimeEntry.DirtyDeleted).Select(te => te.UpdatedAt(timeService.CurrentDateTime));
    }
}
