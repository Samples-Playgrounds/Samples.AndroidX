using System;
using System.Reactive.Linq;
using Toggl.Shared;

namespace Toggl.Core.Sync.States
{
    public sealed class InvalidTransitionState : ISyncState
    {
        private readonly string message;

        public InvalidTransitionState(string message)
        {
            Ensure.Argument.IsNotNullOrWhiteSpaceString(message, nameof(message));

            this.message = message;
        }

        public IObservable<ITransition> Start()
            => Observable.Throw<ITransition>(new InvalidOperationException($"Invalid transition: {message}"));
    }
}
