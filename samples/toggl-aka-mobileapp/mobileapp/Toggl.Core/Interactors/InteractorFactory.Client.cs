using System;
using System.Collections.Generic;
using Toggl.Core.Interactors.Generic;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    public sealed partial class InteractorFactory
    {
        public IInteractor<IObservable<IEnumerable<IThreadSafeClient>>> GetAllClientsInWorkspace(long workspaceId)
            => new GetAllClientsInWorkspaceInteractor(dataSource.Clients, workspaceId);

        public IInteractor<IObservable<IThreadSafeClient>> CreateClient(string clientName, long workspaceId)
            => new CreateClientInteractor(idProvider, timeService, dataSource.Clients, clientName, workspaceId);

        public IInteractor<IObservable<IThreadSafeClient>> GetClientById(long id)
            => new GetByIdInteractor<IThreadSafeClient, IDatabaseClient>(dataSource.Clients, analyticsService, id)
                .TrackException<Exception, IThreadSafeClient>("GetClientById");
    }
}
