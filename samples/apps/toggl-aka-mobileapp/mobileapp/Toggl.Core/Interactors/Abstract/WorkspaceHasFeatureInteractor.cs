using System;
using System.Reactive.Linq;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Interactors
{
    internal abstract class WorkspaceHasFeatureInteractor<TValue> : IInteractor<IObservable<TValue>>
    {
        protected IInteractorFactory InteractorFactory { get; }

        public WorkspaceHasFeatureInteractor(IInteractorFactory interactorFactory)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            InteractorFactory = interactorFactory;
        }

        public IObservable<bool> CheckIfFeatureIsEnabled(long workspaceId, WorkspaceFeatureId featureId)
            => InteractorFactory.GetWorkspaceFeaturesById(workspaceId)
                .Execute()
                .Select(featureCollection => featureCollection.IsEnabled(featureId));

        public abstract IObservable<TValue> Execute();
    }
}
