using Toggl.Core.Services;
using Toggl.iOS.Shared;

namespace Toggl.iOS.Services
{
    public class PrivateSharedStorageServiceIos : IPrivateSharedStorageService
    {
        public void SaveApiToken(string apiToken)
        {
            SharedStorage.Instance.SetApiToken(apiToken);
        }

        public void SaveUserId(long userId)
        {
            SharedStorage.Instance.SetUserId(userId);
        }

        public void SaveDefaultWorkspaceId(long workspaceId)
        {
            SharedStorage.Instance.SetDefaultWorkspaceId(workspaceId);
        }

        public void ClearAll()
        {
            SharedStorage.Instance.DeleteEverything();
        }

        public bool HasUserDataStored()
            => !string.IsNullOrEmpty(SharedStorage.Instance.GetApiToken());

        public string GetApiToken()
            => SharedStorage.Instance.GetApiToken();
    }
}
