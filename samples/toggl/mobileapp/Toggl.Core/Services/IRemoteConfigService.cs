using System;
using Toggl.Shared;

namespace Toggl.Core.Services
{
    public interface IRemoteConfigService
    {
        RatingViewConfiguration GetRatingViewConfiguration();
        PushNotificationsConfiguration GetPushNotificationsConfiguration();
        January2020CampaignConfiguration GetJanuary2020CampaignConfiguration();
    }
}
