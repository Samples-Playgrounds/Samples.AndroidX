using System;
using System.Reactive.Linq;

namespace Toggl.Core.Sync.States
{
    public sealed class DeadEndState : ISyncState
    {
        private static readonly ITransition transition = new StateResult().Transition();

        public IObservable<ITransition> Start() => Observable.Return(transition);
    }
}
