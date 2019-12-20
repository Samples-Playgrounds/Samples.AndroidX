namespace Toggl.Core.Sync
{
    public sealed class StateMachineEntryPoints
    {
        public StateResult StartPullSync { get; } = new StateResult();
        public StateResult StartPushSync { get; } = new StateResult();
        public StateResult StartCleanUp { get; } = new StateResult();
        public StateResult StartPullTimeEntries { get; } = new StateResult();
    }
}
