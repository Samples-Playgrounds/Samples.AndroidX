using System;
using System.Reactive.Linq;
using Toggl.Core.Models.Interfaces;
using Toggl.Networking.Exceptions;

namespace Toggl.Core.Sync.States.Push
{
    public sealed class ProcessClientErrorState<T> : ISyncState<(Exception Error, T Entity)>
        where T : class, IThreadSafeModel
    {
        public StateResult<TooManyRequestsException> UnresolvedTooManyRequests { get; }
            = new StateResult<TooManyRequestsException>();

        public StateResult<(Exception, T)> Unresolved { get; } = new StateResult<(Exception, T)>();

        public IObservable<ITransition> Start((Exception Error, T Entity) parameter)
            => parameter.Error is ClientErrorException == false
                ? Observable.Throw<ITransition>(new ArgumentException(nameof(parameter.Error)))
                : parameter.Error is TooManyRequestsException tooManyRequests
                    ? Observable.Return((ITransition)UnresolvedTooManyRequests.Transition(tooManyRequests))
                    : Observable.Return(Unresolved.Transition(parameter));
    }
}
