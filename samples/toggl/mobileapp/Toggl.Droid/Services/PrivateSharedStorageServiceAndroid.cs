using Toggl.Core.Services;
using Toggl.Droid.Helper;
using Toggl.Droid.Widgets;
using Toggl.Shared;
using Toggl.Storage.Settings;

namespace Toggl.Droid.Services
{
    public class PrivateSharedStorageServiceAndroid : IPrivateSharedStorageService
    {
        private static string apiTokenKey = "SharedStorageApiTokenKey";
        private static string userIdKey = "SharedStorageUserIdKey";
        private static string defaultWorkspaceIdKey = "SharedStorageDefaultWorkspaceIdKey";

        private readonly IKeyValueStorage keyValueStorage;

        public PrivateSharedStorageServiceAndroid(IKeyValueStorage keyValueStorage)
        {
            Ensure.Argument.IsNotNull(keyValueStorage, nameof(keyValueStorage));
            this.keyValueStorage = keyValueStorage;
        }

        public void SaveApiToken(string apiToken)
        {
            keyValueStorage.SetString(apiTokenKey, apiToken);
            updateWidgets();
        }

        public void SaveUserId(long userId)
        {
            keyValueStorage.SetLong(userIdKey, userId);
        }

        public void SaveDefaultWorkspaceId(long workspaceId)
        {
            keyValueStorage.SetLong(defaultWorkspaceIdKey, workspaceId);
        }

        public void ClearAll()
        {
            keyValueStorage.Remove(apiTokenKey);
            keyValueStorage.Remove(userIdKey);
            clearWidgets();
        }

        public bool HasUserDataStored()
            => !string.IsNullOrEmpty(keyValueStorage.GetString(apiTokenKey))
               && keyValueStorage.GetLong(userIdKey, -1L) != -1;

        public string GetApiToken()
            => keyValueStorage.GetString(apiTokenKey);

        private void clearWidgets()
        {
            TimeEntryWidgetInfo.Clear();
            updateWidgets();
        }

        private void updateWidgets()
        {
            AppWidgetProviderUtils.UpdateAllInstances<TimeEntryWidget>();
            AppWidgetProviderUtils.UpdateAllInstances<SuggestionsWidget>();
        }
    }
}
