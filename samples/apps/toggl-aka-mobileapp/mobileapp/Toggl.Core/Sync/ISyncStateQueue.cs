namespace Toggl.Core.Sync
{
    public interface ISyncStateQueue
    {
        void QueuePushSync();
        void QueuePullSync();
        void QueueCleanUp();
        void QueuePullTimeEntries();

        SyncState Dequeue();
        void Clear();
    }
}
