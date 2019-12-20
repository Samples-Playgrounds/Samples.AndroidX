using System;
using Toggl.Core.Interactors.Generic;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    public sealed partial class InteractorFactory : IInteractorFactory
    {
        public IInteractor<IObservable<IThreadSafeWorkspaceFeatureCollection>> GetWorkspaceFeaturesById(long id)
            => new GetByIdInteractor<IThreadSafeWorkspaceFeatureCollection, IDatabaseWorkspaceFeatureCollection>(dataSource.WorkspaceFeatures, analyticsService, id)
                .TrackException<Exception, IThreadSafeWorkspaceFeatureCollection>("GetWorkspaceFeaturesById");
    }
}
