﻿using System;
using System.Collections.Generic;
using System.Reactive;
using Toggl.Core.Interactors.Generic;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    public sealed partial class InteractorFactory : IInteractorFactory
    {
        public IInteractor<IObservable<IThreadSafeWorkspace>> GetDefaultWorkspace()
            => new GetDefaultWorkspaceInteractor(dataSource, this, analyticsService);

        public IInteractor<IObservable<Unit>> SetDefaultWorkspace(long workspaceId)
            => new SetDefaultWorkspaceInteractor(timeService, dataSource.User, workspaceId);

        public IInteractor<IObservable<bool>> AreCustomColorsEnabledForWorkspace(long workspaceId)
            => new AreCustomColorsEnabledForWorkspaceInteractor(this, workspaceId);

        public IInteractor<IObservable<bool?>> AreProjectsBillableByDefault(long workspaceId)
            => new AreProjectsBillableByDefaultInteractor(this, workspaceId);

        public IInteractor<IObservable<IThreadSafeWorkspace>> GetWorkspaceById(long workspaceId)
            => new GetByIdInteractor<IThreadSafeWorkspace, IDatabaseWorkspace>(dataSource.Workspaces, analyticsService, workspaceId)
                .TrackException<Exception, IThreadSafeWorkspace>("GetWorkspaceById");

        public IInteractor<IObservable<IEnumerable<IThreadSafeWorkspace>>> GetAllWorkspaces()
            => new GetAllWorkspacesInteractor(dataSource);

        public IInteractor<IObservable<bool>> WorkspaceAllowsBillableRates(long workspaceId)
            => new WorkspaceAllowsBillableRatesInteractor(this, workspaceId);

        public IInteractor<IObservable<bool>> IsBillableAvailableForWorkspace(long workspaceId)
            => new IsBillableAvailableForWorkspaceInteractor(this, workspaceId);

        public IInteractor<IObservable<Unit>> CreateDefaultWorkspace()
            => new CreateDefaultWorkspaceInteractor(idProvider, timeService, dataSource.User, dataSource.Workspaces, syncManager);

        public IInteractor<IObservable<IEnumerable<IThreadSafeWorkspace>>> ObserveAllWorkspaces()
            => new ObserveAllWorkspacesInteractor(dataSource);

        public IInteractor<IObservable<Unit>> ObserveWorkspacesChanges()
            => new ObserveWorkspacesChangesInteractor(dataSource);

        public IInteractor<IObservable<long>> ObserveDefaultWorkspaceId()
            => new ObserveDefaultWorkspaceIdInteractor(dataSource);
    }
}
