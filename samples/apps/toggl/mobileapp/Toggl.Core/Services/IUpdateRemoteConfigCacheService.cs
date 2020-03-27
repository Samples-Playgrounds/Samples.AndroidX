using System;
using System.Reactive;

namespace Toggl.Core.Services
{
    public interface IUpdateRemoteConfigCacheService
    {
        IObservable<Unit> RemoteConfigChanged { get; }
        void FetchAndStoreRemoteConfigData();
        bool NeedsToUpdateStoredRemoteConfigData();
    }

    public static class RemoteConfigKeys
    {
        public static string RatingViewDelayParameter { get; } = "UserRatingDialogDelayInDays";
        public static string RatingViewTriggerParameter { get; } = "UserRatingDialogTrigger";
        public static string RegisterPushNotificationsTokenWithServerParameter { get; } = "PushNotificationsRegisterTokenWithServer";
        public static string HandlePushNotificationsParameter { get; } = "PushNotificationsHandle";
        public static string LastFetchAtKey { get; } = "LastFetchAtKey";
        public static string January2020CampaignOption { get; } = "January2020CampaignOption";
    }
}
