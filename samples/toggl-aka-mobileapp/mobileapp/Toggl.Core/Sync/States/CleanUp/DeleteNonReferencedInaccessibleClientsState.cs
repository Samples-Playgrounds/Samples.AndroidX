using System;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Storage.Models;

namespace Toggl.Core.Sync.States.CleanUp
{
    public sealed class DeleteNonReferencedInaccessibleClientsState : DeleteInaccessibleEntityState<IThreadSafeClient, IDatabaseClient>
    {
        private readonly IDataSource<IThreadSafeProject, IDatabaseProject> projectsDataSource;

        public DeleteNonReferencedInaccessibleClientsState(
            IDataSource<IThreadSafeClient, IDatabaseClient> clientsDataSource,
            IDataSource<IThreadSafeProject, IDatabaseProject> projectsDataSource
        ) : base(clientsDataSource)
        {
            Ensure.Argument.IsNotNull(projectsDataSource, nameof(projectsDataSource));
            this.projectsDataSource = projectsDataSource;
        }

        protected override IObservable<bool> SuitableForDeletion(IThreadSafeClient client)
            => projectsDataSource.GetAll(
                    project => isReferenced(client, project),
                    includeInaccessibleEntities: true)
                .Select(references => references.None());

        private bool isReferenced(IClient client, IProject project)
            => project.Id == client.Id;
    }
}
