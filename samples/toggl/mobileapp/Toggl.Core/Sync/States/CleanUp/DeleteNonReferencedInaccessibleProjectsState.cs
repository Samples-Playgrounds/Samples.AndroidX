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
    public sealed class DeleteNonReferencedInaccessibleProjectsState
        : DeleteInaccessibleEntityState<IThreadSafeProject, IDatabaseProject>
    {
        private readonly IDataSource<IThreadSafeTask, IDatabaseTask> tasksDataSource;
        private readonly IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> timeEntriesDataSource;

        public DeleteNonReferencedInaccessibleProjectsState(
            IDataSource<IThreadSafeProject, IDatabaseProject> projectsDataSource,
            IDataSource<IThreadSafeTask, IDatabaseTask> tasksDataSource,
            IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> timeEntriesDataSource
        ) : base(projectsDataSource)
        {
            Ensure.Argument.IsNotNull(tasksDataSource, nameof(tasksDataSource));
            Ensure.Argument.IsNotNull(timeEntriesDataSource, nameof(timeEntriesDataSource));

            this.tasksDataSource = tasksDataSource;
            this.timeEntriesDataSource = timeEntriesDataSource;
        }

        protected override IObservable<bool> SuitableForDeletion(IThreadSafeProject project)
            => Observable.CombineLatest(
                isReferencedByAnyTask(project),
                isReferencedByAnyTimeEntry(project),
                (refByTasks, refByTimeEntries) => !(refByTasks || refByTimeEntries));

        private IObservable<bool> isReferencedByAnyTask(IThreadSafeProject project)
            => tasksDataSource.GetAll(
                    task => isReferenced(project, task),
                    includeInaccessibleEntities: true)
                .Select(references => references.Any());

        private IObservable<bool> isReferencedByAnyTimeEntry(IThreadSafeProject project)
            => timeEntriesDataSource.GetAll(
                    timeEntry => isReferenced(project, timeEntry),
                    includeInaccessibleEntities: true)
                .Select(references => references.Any());

        private bool isReferenced(IProject project, ITask task)
            => task.ProjectId == project.Id;

        private bool isReferenced(IProject project, ITimeEntry timeEntry)
            => timeEntry.ProjectId == project.Id;
    }
}
