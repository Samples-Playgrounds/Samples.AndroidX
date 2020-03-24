using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Models;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Core.Interactors
{
    internal sealed class DeleteTimeEntryInteractor : IInteractor<Task>
    {
        private readonly long id;
        private readonly ITimeService timeService;
        private readonly IObservableDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> dataSource;
        private readonly IInteractorFactory interactorFactory;

        public DeleteTimeEntryInteractor(
            ITimeService timeService,
            IObservableDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> dataSource,
            IInteractorFactory interactorFactory,
            long id)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.id = id;
            this.dataSource = dataSource;
            this.timeService = timeService;
            this.interactorFactory = interactorFactory;
        }

        public Task Execute()
            => Task.Run(async () =>
            {
                var timeEntryId = await interactorFactory.GetTimeEntryById(id).Execute(); 
                var timeEntry = TimeEntry.DirtyDeleted(timeEntryId);
                timeEntry.UpdatedAt(timeService.CurrentDateTime);
                await dataSource.Update(timeEntry);
            });
    }
}
