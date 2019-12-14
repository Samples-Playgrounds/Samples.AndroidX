using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Storage.Models;

namespace Toggl.Core.Sync.States.CleanUp
{
    public sealed class DeleteNonReferencedInaccessibleTasksState : DeleteInaccessibleEntityState<IThreadSafeTask, IDatabaseTask>
    {
        private IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> timeEntriesDataSource;

        public DeleteNonReferencedInaccessibleTasksState(
            IDataSource<IThreadSafeTask, IDatabaseTask> tasksDataSource,
            IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> timeEntriesDataSource
            ) : base(tasksDataSource)
        {
            Ensure.Argument.IsNotNull(timeEntriesDataSource, nameof(timeEntriesDataSource));
            this.timeEntriesDataSource = timeEntriesDataSource;
        }

        protected override IObservable<bool> SuitableForDeletion(IThreadSafeTask task)
            => timeEntriesDataSource.GetAll(
                    timeEntry => isReferenced(task, timeEntry),
                    includeInaccessibleEntities: true)
                .Select(references => references.None());

        private bool isReferenced(ITask task, ITimeEntry timeEntry)
            => timeEntry.TaskId == task.Id;
    }
}
