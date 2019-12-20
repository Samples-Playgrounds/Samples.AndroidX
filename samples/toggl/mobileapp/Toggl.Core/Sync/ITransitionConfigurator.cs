namespace Toggl.Core.Sync
{
    public interface ITransitionConfigurator
    {
        void ConfigureTransition(IStateResult result, ISyncState state);
        void ConfigureTransition<T>(StateResult<T> result, ISyncState<T> state);
    }
}
