namespace Toggl.Storage
{
    public enum SyncStatus
    {
        InSync = 0,
        SyncNeeded = 1,
        SyncFailed = 2,
        RefetchingNeeded = 3
    }
}
