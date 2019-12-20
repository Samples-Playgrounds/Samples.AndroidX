using System;
using System.Reactive.Linq;
using Toggl.Core.Extensions;
using Toggl.Networking.Exceptions;

namespace Toggl.Core.Sync.States.Pull
{
    public abstract class EnsureFetchSucceededState<T> : ISyncState<IFetchObservables>
    {
        public StateResult<IFetchObservables> Done { get; } = new StateResult<IFetchObservables>();

        public StateResult<ApiException> ErrorOccured { get; } = new StateResult<ApiException>();

        public IObservable<ITransition> Start(IFetchObservables fetchObservables)
            => FetchObservable(fetchObservables)
                .SingleAsync()
                .Select(_ => Done.Transition(fetchObservables))
                .OnErrorReturnResult(ErrorOccured);

        protected abstract IObservable<T> FetchObservable(IFetchObservables fetch);
    }
}
