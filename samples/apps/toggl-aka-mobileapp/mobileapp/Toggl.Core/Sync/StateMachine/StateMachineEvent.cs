using System;

namespace Toggl.Core.Sync
{
    public abstract class StateMachineEvent
    {
        public abstract override string ToString();
    }

    internal sealed class StateMachineTransition : StateMachineEvent
    {
        public ITransition Transition { get; }

        public StateMachineTransition(ITransition transition)
        {
            Transition = transition;
        }

        public override string ToString() => "State machine is transitioning to new state.";
    }

    internal sealed class StateMachineDeadEnd : StateMachineEvent
    {
        public ITransition Transition { get; }

        public StateMachineDeadEnd(ITransition transition)
        {
            Transition = transition;
        }

        public override string ToString() => "State machine reached dead end.";
    }

    internal sealed class StateMachineError : StateMachineEvent
    {
        public Exception Exception { get; }

        public StateMachineError(Exception exception)
        {
            Exception = exception;
        }

        public override string ToString() => $"State machine encountered an error: {Exception.Message}";
    }
}
