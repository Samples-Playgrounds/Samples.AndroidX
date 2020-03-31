using static Toggl.Core.Sync.SyncState;

namespace Toggl.Core.Sync
{
    internal sealed class SyncStateQueue : ISyncStateQueue
    {
        private bool pulledLast;
        private bool pullSyncQueued;
        private bool pushSyncQueued;
        private bool cleanUpQueued;
        private bool pullTimeEntriesQueued;

        public void QueuePushSync()
        {
            pushSyncQueued = true;
        }

        public void QueuePullSync()
        {
            pullSyncQueued = true;
        }

        public void QueueCleanUp()
        {
            cleanUpQueued = true;
        }

        public void QueuePullTimeEntries()
        {
            pullTimeEntriesQueued = true;
        }

        public SyncState Dequeue()
        {
            if (pullTimeEntriesQueued)
                return pullTimeEntries();

            if (pulledLast)
                return push();

            if (pullSyncQueued)
                return pull();

            if (pushSyncQueued)
                return push();

            if (cleanUpQueued)
                return cleanUp();

            return Sleep;
        }

        public void Clear()
        {
            pulledLast = false;
            pullSyncQueued = false;
            pushSyncQueued = false;
            cleanUpQueued = false;
            pullTimeEntriesQueued = false;
        }

        private SyncState pull()
        {
            pullSyncQueued = false;
            pulledLast = true;
            return Pull;
        }

        private SyncState push()
        {
            pushSyncQueued = false;
            pulledLast = false;
            return Push;
        }

        private SyncState cleanUp()
        {
            cleanUpQueued = false;
            return CleanUp;
        }

        private SyncState pullTimeEntries()
        {
            pullTimeEntriesQueued = false;
            return PullTimeEntries;
        }
    }
}
