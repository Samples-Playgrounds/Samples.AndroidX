using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Storage.Settings;

namespace Toggl.Core.Extensions
{
    public static class RemoteConfigExtensions
    {
        public static RatingViewConfiguration ReadStoredRatingViewConfiguration(this IKeyValueStorage keyValueStorage)
            => new RatingViewConfiguration(
                keyValueStorage.GetInt(RemoteConfigKeys.RatingViewDelayParameter, 5),
                (keyValueStorage.GetString(RemoteConfigKeys.RatingViewTriggerParameter) ?? string.Empty).ToRatingViewCriterion());

        public static PushNotificationsConfiguration ReadStoredPushNotificationsConfiguration(this IKeyValueStorage keyValueStorage)
            => new PushNotificationsConfiguration(
                keyValueStorage.GetBool(RemoteConfigKeys.RegisterPushNotificationsTokenWithServerParameter),
                keyValueStorage.GetBool(RemoteConfigKeys.HandlePushNotificationsParameter));

        public static January2020CampaignConfiguration ReadJanuary2020CampaignConfiguration(this IKeyValueStorage keyValueStorage)
            => new January2020CampaignConfiguration(
                keyValueStorage.GetString(RemoteConfigKeys.January2020CampaignOption));
    }
}
