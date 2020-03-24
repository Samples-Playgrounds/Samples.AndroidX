using System;
using System.Reactive.Linq;
using static Toggl.Shared.WorkspaceFeatureId;

namespace Toggl.Core.Interactors
{
    internal sealed class IsBillableAvailableForProjectInteractor : WorkspaceHasFeatureInteractor<bool>
    {
        private readonly long projectId;

        public IsBillableAvailableForProjectInteractor(IInteractorFactory interactorFactory, long projectId)
            : base(interactorFactory)
        {
            this.projectId = projectId;
        }

        public override IObservable<bool> Execute()
            => InteractorFactory.GetProjectById(projectId)
                .Execute()
                .SelectMany(project => CheckIfFeatureIsEnabled(project.WorkspaceId, Pro));
    }
}
