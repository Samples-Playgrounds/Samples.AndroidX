using Toggl.Core.Sync;

namespace Toggl.Core.Services
{
    public interface ISyncErrorHandlingService
    {
        void HandleErrorsOf(ISyncManager syncManager);
    }
}
