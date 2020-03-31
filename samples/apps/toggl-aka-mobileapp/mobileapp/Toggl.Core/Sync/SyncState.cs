namespace Toggl.Core.Sync
{
    public enum SyncState
    {
        Sleep = 0,
        Pull,
        Push,
        CleanUp,
        PullTimeEntries
    }
}
