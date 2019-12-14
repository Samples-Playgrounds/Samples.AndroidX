using Toggl.Core.Extensions;
using Toggl.Shared;
using Toggl.Storage.Settings;

namespace Toggl.Core.Services
{
    public class RemoteConfigService : IRemoteConfigService
    {
        private readonly IKeyValueStorage keyValueStorage;

        public RemoteConfigService(IKeyValueStorage keyValueStorage)
        {
            Ensure.Argument.IsNotNull(keyValueStorage, nameof(keyValueStorage));
            this.keyValueStorage = keyValueStorage;
        }

        public RatingViewConfiguration GetRatingViewConfiguration()
            => keyValueStorage.ReadStoredRatingViewConfiguration();

        public PushNotificationsConfiguration GetPushNotificationsConfiguration()
            => keyValueStorage.ReadStoredPushNotificationsConfiguration();

        public January2020CampaignConfiguration GetJanuary2020CampaignConfiguration()
            => keyValueStorage.ReadJanuary2020CampaignConfiguration();
    }
}
