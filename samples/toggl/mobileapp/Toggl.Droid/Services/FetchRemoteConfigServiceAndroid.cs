using System;
using System.Diagnostics;
using Android.Gms.Tasks;
using Firebase.RemoteConfig;
using Toggl.Core.Services;
using Toggl.Shared;
using static Toggl.Core.Services.RemoteConfigKeys;
using GmsTask = Android.Gms.Tasks.Task;

namespace Toggl.Droid.Services
{
    public class FetchRemoteConfigServiceAndroid : IFetchRemoteConfigService
    {
        [Conditional("DEBUG")]
        private void enableDeveloperModeInDebugModel(FirebaseRemoteConfig remoteConfig)
        {
            var settings = new FirebaseRemoteConfigSettings
                    .Builder()
                .SetDeveloperModeEnabled(true)
                .Build();

            remoteConfig.SetConfigSettings(settings);
        }

        public void FetchRemoteConfigData(Action onFetchSucceeded, Action<Exception> onFetchFailed)
        {
            var remoteConfig = FirebaseRemoteConfig.Instance;
            enableDeveloperModeInDebugModel(remoteConfig);
            remoteConfig.SetDefaults(Resource.Xml.RemoteConfigDefaults);
            
            remoteConfig.Fetch(error =>
            {
                if (error != null)
                    onFetchFailed(error);
                else
                {
                    remoteConfig.ActivateFetched();
                    onFetchSucceeded();
                }
            });
        }

        public RatingViewConfiguration ExtractRatingViewConfigurationFromRemoteConfig()
        {
            var remoteConfig = FirebaseRemoteConfig.Instance;
            return new RatingViewConfiguration(
                (int) remoteConfig.GetValue(RatingViewDelayParameter).AsLong(),
                remoteConfig.GetString(RatingViewTriggerParameter).ToRatingViewCriterion());
        }

        public PushNotificationsConfiguration ExtractPushNotificationsConfigurationFromRemoteConfig()
        {
            var remoteConfig = FirebaseRemoteConfig.Instance;
            return new PushNotificationsConfiguration(
                remoteConfig.GetBoolean(RegisterPushNotificationsTokenWithServerParameter),
                remoteConfig.GetBoolean(HandlePushNotificationsParameter));
        }

        public January2020CampaignConfiguration ExtractJanuary2020CampaignConfig()
        {
            var remoteConfig = FirebaseRemoteConfig.Instance;
            return new January2020CampaignConfiguration(
                remoteConfig.GetString(January2020CampaignOption));
        }
    }

    public class RemoteConfigCompletionHandler : Java.Lang.Object, IOnCompleteListener
    {
        private readonly Action<Exception> action;
        
        public RemoteConfigCompletionHandler(Action<Exception> action)
        {
            this.action = action;
        }

        public void OnComplete(GmsTask task)
        {
            action(task.IsSuccessful ? null : task.Exception);
        }
    }

    public static class FirebaseExtensions
    {
        public static void Fetch(this FirebaseRemoteConfig remoteConfig, Action<Exception> action)
        {
            remoteConfig.Fetch().AddOnCompleteListener(new RemoteConfigCompletionHandler(action));
        }
    }
}
