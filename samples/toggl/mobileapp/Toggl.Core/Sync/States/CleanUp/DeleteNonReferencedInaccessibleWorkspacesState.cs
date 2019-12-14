using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage.Models;

namespace Toggl.Core.Sync.States.CleanUp
{
    public sealed class DeleteNonReferencedInaccessibleWorkspacesState
        : DeleteInaccessibleEntityState<IThreadSafeWorkspace, IDatabaseWorkspace>
    {
        private readonly IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> timeEntriesDataSource;
        private readonly IDataSource<IThreadSafeProject, IDatabaseProject> projectsDataSource;
        private readonly IDataSource<IThreadSafeTask, IDatabaseTask> tasksDataSource;
        private readonly IDataSource<IThreadSafeClient, IDatabaseClient> clientsDataSource;
        private readonly IDataSource<IThreadSafeTag, IDatabaseTag> tagsDataSource;

        public DeleteNonReferencedInaccessibleWorkspacesState(
            IDataSource<IThreadSafeWorkspace, IDatabaseWorkspace> workspacesDataSource,
            IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> timeEntriesDataSource,
            IDataSource<IThreadSafeProject, IDatabaseProject> projectsDataSource,
            IDataSource<IThreadSafeTask, IDatabaseTask> tasksDataSource,
            IDataSource<IThreadSafeClient, IDatabaseClient> clientsDataSource,
            IDataSource<IThreadSafeTag, IDatabaseTag> tagsDataSource
        ) : base(workspacesDataSource)
        {
            Ensure.Argument.IsNotNull(timeEntriesDataSource, nameof(timeEntriesDataSource));
            Ensure.Argument.IsNotNull(projectsDataSource, nameof(projectsDataSource));
            Ensure.Argument.IsNotNull(tasksDataSource, nameof(tasksDataSource));
            Ensure.Argument.IsNotNull(clientsDataSource, nameof(clientsDataSource));
            Ensure.Argument.IsNotNull(tagsDataSource, nameof(tagsDataSource));

            this.timeEntriesDataSource = timeEntriesDataSource;
            this.projectsDataSource = projectsDataSource;
            this.tasksDataSource = tasksDataSource;
            this.clientsDataSource = clientsDataSource;
            this.tagsDataSource = tagsDataSource;
        }

        protected override IObservable<bool> SuitableForDeletion(IThreadSafeWorkspace workspace)
            => Observable.CombineLatest(
                isReferenced(workspace, timeEntriesDataSource, (ws, timeEntry) => timeEntry.WorkspaceId == ws.Id),
                isReferenced(workspace, projectsDataSource, (ws, project) => project.WorkspaceId == ws.Id),
                isReferenced(workspace, tasksDataSource, (ws, task) => task.WorkspaceId == ws.Id),
                isReferenced(workspace, clientsDataSource, (ws, client) => client.WorkspaceId == ws.Id),
                isReferenced(workspace, tagsDataSource, (ws, tag) => tag.WorkspaceId == ws.Id),
                (timeEntries, projects, tasks, clients, tags) => !(timeEntries || projects || tasks || clients || tags));

        private IObservable<bool> isReferenced<TInterface, TDatabaseInterface>(
            IThreadSafeWorkspace workspace,
            IDataSource<TInterface, TDatabaseInterface> dataSource,
            Func<IThreadSafeWorkspace, TDatabaseInterface, bool> isReferenced)
            where TInterface : TDatabaseInterface, IThreadSafeModel
            where TDatabaseInterface : IDatabaseModel
            => dataSource.GetAll(
                entity => isReferenced(workspace, entity),
                includeInaccessibleEntities: true)
            .Select(references => references.Any());
    }
}
