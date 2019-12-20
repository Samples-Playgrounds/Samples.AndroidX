namespace Toggl.Core.Sync
{
    public interface IStateResult { }

    public sealed class StateResult : IStateResult
    {
        private readonly Transition singletonTransition;

        public StateResult()
        {
            singletonTransition = new Transition(this);
        }

        public Transition Transition() => singletonTransition;
    }

    public sealed class StateResult<T> : IStateResult
    {
        public Transition<T> Transition(T parameter) => new Transition<T>(this, parameter);
    }
}
