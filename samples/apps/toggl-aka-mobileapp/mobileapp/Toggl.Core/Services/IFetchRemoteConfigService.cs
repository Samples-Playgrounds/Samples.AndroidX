using System;
using Toggl.Shared;

namespace Toggl.Core.Services
{
    public interface IFetchRemoteConfigService
    {
        void FetchRemoteConfigData(Action onFetchSucceeded, Action<Exception> onFetchFailed);
        RatingViewConfiguration ExtractRatingViewConfigurationFromRemoteConfig();
        PushNotificationsConfiguration ExtractPushNotificationsConfigurationFromRemoteConfig();
        January2020CampaignConfiguration ExtractJanuary2020CampaignConfig();
    }
}