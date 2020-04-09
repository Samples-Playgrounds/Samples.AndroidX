using Toggl.Core.Login;

namespace Toggl.Core.Services
{
    public interface IBackgroundSyncService
    {
        void SetupBackgroundSync(IUserAccessManager loginManager);
    }
}
