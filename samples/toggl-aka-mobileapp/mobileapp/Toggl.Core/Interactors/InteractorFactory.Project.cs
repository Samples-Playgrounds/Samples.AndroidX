using System;
using Toggl.Core.DTOs;
using Toggl.Core.Interactors.Generic;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    public sealed partial class InteractorFactory : IInteractorFactory
    {
        public IInteractor<IObservable<bool>> IsBillableAvailableForProject(long projectId)
            => new IsBillableAvailableForProjectInteractor(this, projectId);

        public IInteractor<IObservable<bool>> ProjectDefaultsToBillable(long projectId)
            => new ProjectDefaultsToBillableInteractor(this, projectId);

        public IInteractor<IObservable<IThreadSafeProject>> CreateProject(CreateProjectDTO dto)
            => new CreateProjectInteractor(idProvider, timeService, dataSource.Projects, dto);

        public IInteractor<IObservable<IThreadSafeProject>> GetProjectById(long id)
            => new GetByIdInteractor<IThreadSafeProject, IDatabaseProject>(dataSource.Projects, analyticsService, id)
                .TrackException<Exception, IThreadSafeProject>("GetProjectById");
    }
}
