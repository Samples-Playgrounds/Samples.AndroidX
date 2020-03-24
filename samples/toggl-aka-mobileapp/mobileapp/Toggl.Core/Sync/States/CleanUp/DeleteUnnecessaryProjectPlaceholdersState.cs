using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Sync.States.CleanUp
{
    public sealed class DeleteUnnecessaryProjectPlaceholdersState : ISyncState
    {
        private readonly IDataSource<IThreadSafeProject, IDatabaseProject> projectsDataSource;

        private readonly IObservableDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> timeEntriesDataSource;

        public StateResult Done { get; } = new StateResult();

        public DeleteUnnecessaryProjectPlaceholdersState(IDataSource<IThreadSafeProject, IDatabaseProject> projectsDataSource, IObservableDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> timeEntriesDataSource)
        {
            Ensure.Argument.IsNotNull(projectsDataSource, nameof(projectsDataSource));
            Ensure.Argument.IsNotNull(timeEntriesDataSource, nameof(timeEntriesDataSource));

            this.projectsDataSource = projectsDataSource;
            this.timeEntriesDataSource = timeEntriesDataSource;
        }

        public IObservable<ITransition> Start()
            => projectsDataSource.GetAll(project => project.SyncStatus == SyncStatus.RefetchingNeeded)
                .Flatten()
                .SelectMany(notReferencedByAnyTimeEntryOrNull)
                .Where(project => project != null)
                .ToList()
                .SelectMany(projectsDataSource.DeleteAll)
                .SelectValue(Done.Transition());

        private IObservable<IThreadSafeProject> notReferencedByAnyTimeEntryOrNull(IThreadSafeProject project)
            => timeEntriesDataSource.GetAll(timeEntry => timeEntry.ProjectId == project.Id)
                .Select(referencingTimeEntries => referencingTimeEntries.Any())
                .Select(isReferencedByAnyTimeEntry => isReferencedByAnyTimeEntry ? null : project);
    }
}
