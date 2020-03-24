using System;
using static Toggl.Shared.WorkspaceFeatureId;

namespace Toggl.Core.Interactors
{
    internal sealed class AreCustomColorsEnabledForWorkspaceInteractor : WorkspaceHasFeatureInteractor<bool>
    {
        private readonly long workspaceId;

        public AreCustomColorsEnabledForWorkspaceInteractor(IInteractorFactory interactorFactory, long workspaceId)
            : base(interactorFactory)
        {
            this.workspaceId = workspaceId;
        }

        public override IObservable<bool> Execute()
            => CheckIfFeatureIsEnabled(workspaceId, Pro);
    }
}
