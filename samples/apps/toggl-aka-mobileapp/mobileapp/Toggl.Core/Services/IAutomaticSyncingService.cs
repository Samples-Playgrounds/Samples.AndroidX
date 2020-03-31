using Toggl.Core.Sync;

namespace Toggl.Core.Services
{
    public interface IAutomaticSyncingService
    {
        void Start(ISyncManager syncManager);
        void Stop();
    }
}
