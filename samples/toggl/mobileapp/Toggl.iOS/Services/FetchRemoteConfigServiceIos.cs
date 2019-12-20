using System;
using Firebase.RemoteConfig;
using Toggl.Core.Services;
using Toggl.Shared;
using static Toggl.Core.Services.RemoteConfigKeys;

namespace Toggl.iOS.Services
{
    public class FetchRemoteConfigServiceIos : IFetchRemoteConfigService
    {
        private const string remoteConfigDefaultsFileName = "RemoteConfigDefaults";
        
        public FetchRemoteConfigServiceIos()
        {
#if !DEBUG
            RemoteConfig.SharedInstance.SetDefaults(plistFileName: remoteConfigDefaultsFileName);
#endif
        }

        public void FetchRemoteConfigData(Action onFetchSucceeded, Action<Exception> onFetchFailed)
        {
            var remoteConfig = RemoteConfig.SharedInstance;

            remoteConfig.FetchAndActivate((status, error) =>
            {
                if (status == RemoteConfigFetchAndActivateStatus.Error)
                {
                    onFetchFailed(new Exception(error?.Description ?? "Fetching Firebase Remote Configuration failed for some unknown reason."));
                    return;
                }

                onFetchSucceeded();
            });
        }

        public RatingViewConfiguration ExtractRatingViewConfigurationFromRemoteConfig()
        {
            var remoteConfig = RemoteConfig.SharedInstance;
            return new RatingViewConfiguration(
                remoteConfig[RatingViewDelayParameter].NumberValue.Int32Value,
                remoteConfig[RatingViewTriggerParameter].StringValue.ToRatingViewCriterion());
        }

        public PushNotificationsConfiguration ExtractPushNotificationsConfigurationFromRemoteConfig()
        {
            var remoteConfig = RemoteConfig.SharedInstance;
            return new PushNotificationsConfiguration(
                remoteConfig[RegisterPushNotificationsTokenWithServerParameter].BoolValue,
                remoteConfig[HandlePushNotificationsParameter].BoolValue);
        }

        public January2020CampaignConfiguration ExtractJanuary2020CampaignConfig()
        {
            var remoteConfig = RemoteConfig.SharedInstance;
            return new January2020CampaignConfiguration(
                remoteConfig[January2020CampaignOption].StringValue);
        }
    }
}
