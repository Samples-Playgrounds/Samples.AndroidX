using System;

namespace Toggl.Core.Sync
{
    public delegate IObservable<ITransition> TransitionHandler(ITransition transition);

    public interface ITransitionHandlerProvider
    {
        TransitionHandler GetTransitionHandler(IStateResult stateResult);
    }
}
