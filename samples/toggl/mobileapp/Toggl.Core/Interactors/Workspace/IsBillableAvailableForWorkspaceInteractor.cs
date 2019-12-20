﻿using System;
using static Toggl.Shared.WorkspaceFeatureId;

namespace Toggl.Core.Interactors
{
    internal sealed class IsBillableAvailableForWorkspaceInteractor : WorkspaceHasFeatureInteractor<bool>
    {
        private readonly long workspaceId;

        public IsBillableAvailableForWorkspaceInteractor(IInteractorFactory interactorFactory, long workspaceId)
            : base(interactorFactory)
        {
            this.workspaceId = workspaceId;
        }

        public override IObservable<bool> Execute()
            => CheckIfFeatureIsEnabled(workspaceId, Pro);
    }
}
