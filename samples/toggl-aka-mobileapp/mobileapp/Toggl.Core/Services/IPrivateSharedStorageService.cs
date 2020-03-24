namespace Toggl.Core.Services
{
    public interface IPrivateSharedStorageService
    {
        void SaveApiToken(string apiToken);

        void SaveUserId(long userId);

        void SaveDefaultWorkspaceId(long workspaceId);

        void ClearAll();

        bool HasUserDataStored();

        string GetApiToken();
    }
}
