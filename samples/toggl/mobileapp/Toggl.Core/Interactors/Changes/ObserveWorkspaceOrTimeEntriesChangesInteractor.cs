using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Shared;

namespace Toggl.Core.Interactors.Changes
{
    public class ObserveWorkspaceOrTimeEntriesChangesInteractor : IInteractor<IObservable<Unit>>
    {
        private readonly IInteractorFactory interactorFactory;

        public ObserveWorkspaceOrTimeEntriesChangesInteractor(IInteractorFactory interactorFactory)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.interactorFactory = interactorFactory;
        }

        public IObservable<Unit> Execute()
            => Observable.Merge(
                interactorFactory.ObserveWorkspacesChanges().Execute(),
                interactorFactory.ObserveTimeEntriesChanges().Execute());
    }
}
