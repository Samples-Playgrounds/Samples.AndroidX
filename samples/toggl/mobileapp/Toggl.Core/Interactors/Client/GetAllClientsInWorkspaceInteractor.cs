using System;
using System.Collections.Generic;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    internal class GetAllClientsInWorkspaceInteractor : IInteractor<IObservable<IEnumerable<IThreadSafeClient>>>
    {
        private readonly long workspaceId;
        private readonly IDataSource<IThreadSafeClient, IDatabaseClient> dataSource;

        public GetAllClientsInWorkspaceInteractor(IDataSource<IThreadSafeClient, IDatabaseClient> dataSource, long workspaceId)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(workspaceId, nameof(workspaceId));

            this.dataSource = dataSource;
            this.workspaceId = workspaceId;
        }

        public IObservable<IEnumerable<IThreadSafeClient>> Execute()
            => dataSource.GetAll(c => c.WorkspaceId == workspaceId);
    }
}