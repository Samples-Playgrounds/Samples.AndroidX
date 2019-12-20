using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    internal sealed class DeleteMultipleTimeEntriesInteractor : IInteractor<Task>
    {
        private readonly long[] ids;
        private readonly IObservableDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> dataSource;
        private readonly IInteractorFactory interactorFactory;

        public DeleteMultipleTimeEntriesInteractor(
            IObservableDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> dataSource,
            IInteractorFactory interactorFactory,
            long[] ids)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.ids = ids;
            this.dataSource = dataSource;
            this.interactorFactory = interactorFactory;
        }

        public Task Execute()
            => Task.Run(async () =>
            {
                var timeEntriesToDelete = await interactorFactory.GetMultipleTimeEntriesById(ids).Execute(); 
                await dataSource.DeleteAll(timeEntriesToDelete);
            });
    }
}
