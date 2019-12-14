using System;

namespace Toggl.Core.Sync
{
    public interface IStateMachine
    {
        IObservable<StateMachineEvent> StateTransitions { get; }
        void Start(ITransition transition);
        void Freeze();
    }
}
