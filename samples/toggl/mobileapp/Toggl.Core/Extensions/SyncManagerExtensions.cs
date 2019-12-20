using Toggl.Core.Sync;

namespace Toggl.Core.Extensions
{
    public static class SyncManagerExtensions
    {
        public static void InitiatePushSync(this ISyncManager syncManager)
        {
            syncManager.PushSync();
        }
    }
}
