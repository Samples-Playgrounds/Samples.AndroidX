using System;
using Toggl.Core.Interactors.Generic;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    public sealed partial class InteractorFactory : IInteractorFactory
    {
        public IInteractor<IObservable<IThreadSafeTask>> GetTaskById(long id)
            => new GetByIdInteractor<IThreadSafeTask, IDatabaseTask>(dataSource.Tasks, analyticsService, id)
                .TrackException<Exception, IThreadSafeTask>("GetTaskById");
    }
}
