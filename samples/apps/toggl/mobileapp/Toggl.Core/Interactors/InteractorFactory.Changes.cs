using System;
using System.Reactive;
using Toggl.Core.Interactors.Changes;

namespace Toggl.Core.Interactors
{
    public sealed partial class InteractorFactory : IInteractorFactory
    {
        public IInteractor<IObservable<Unit>> ObserveWorkspaceOrTimeEntriesChanges()
            => new ObserveWorkspaceOrTimeEntriesChangesInteractor(this);
    }
}
