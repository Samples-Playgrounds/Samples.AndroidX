using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Shared;
using Toggl.Storage.Settings;
using static Toggl.Core.Services.RemoteConfigKeys;

namespace Toggl.Core.Services
{
    public sealed class UpdateRemoteConfigCacheService : IUpdateRemoteConfigCacheService
    {
        public static readonly TimeSpan RemoteConfigExpiration = TimeSpan.FromHours(12.5f);

        private readonly object updateLock = new object();
        private readonly ITimeService timeService;
        private readonly IKeyValueStorage keyValueStorage;
        private readonly IFetchRemoteConfigService fetchRemoteConfigService;
        private readonly ISubject<Unit> remoteConfigUpdatedSubject = new BehaviorSubject<Unit>(Unit.Default);
        private bool isRunning;

        public IObservable<Unit> RemoteConfigChanged => remoteConfigUpdatedSubject.AsObservable();

        public UpdateRemoteConfigCacheService(ITimeService timeService, IKeyValueStorage keyValueStorage, IFetchRemoteConfigService fetchRemoteConfigService)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(keyValueStorage, nameof(keyValueStorage));
            Ensure.Argument.IsNotNull(fetchRemoteConfigService, nameof(fetchRemoteConfigService));

            this.timeService = timeService;
            this.keyValueStorage = keyValueStorage;
            this.fetchRemoteConfigService = fetchRemoteConfigService;
        }

        public void FetchAndStoreRemoteConfigData()
        {
            lock (updateLock)
            {
                if (isRunning) return;
                isRunning = true;
            }

            fetchRemoteConfigService.FetchRemoteConfigData(onFetchSucceeded, onFetchFailed);
        }

        public bool NeedsToUpdateStoredRemoteConfigData()
        {
            lock (updateLock)
            {
                var lastFetchAt = keyValueStorage.GetDateTimeOffset(LastFetchAtKey);
                if (!lastFetchAt.HasValue) return true;

                var now = timeService.CurrentDateTime;
                return now.Subtract(lastFetchAt.Value) > RemoteConfigExpiration;
            }
        }

        private void onFetchSucceeded()
        {
            var ratingViewConfiguration = fetchRemoteConfigService.ExtractRatingViewConfigurationFromRemoteConfig();
            var pushNotificationsConfiguration = fetchRemoteConfigService.ExtractPushNotificationsConfigurationFromRemoteConfig();
            var january2020CampaignConfiguration = fetchRemoteConfigService.ExtractJanuary2020CampaignConfig();

            keyValueStorage.SetInt(RatingViewDelayParameter, ratingViewConfiguration.DayCount);
            keyValueStorage.SetString(RatingViewTriggerParameter, ratingViewConfiguration.Criterion.ToString());

            keyValueStorage.SetBool(RegisterPushNotificationsTokenWithServerParameter, pushNotificationsConfiguration.RegisterPushNotificationsTokenWithServer);
            keyValueStorage.SetBool(HandlePushNotificationsParameter, pushNotificationsConfiguration.HandlePushNotifications);

            keyValueStorage.SetString(January2020CampaignOption, january2020CampaignConfiguration.Option.ToString());

            lock (updateLock)
            {
                keyValueStorage.SetDateTimeOffset(LastFetchAtKey, timeService.CurrentDateTime);
                remoteConfigUpdatedSubject.OnNext(Unit.Default);
                isRunning = false;
            }
        }

        private void onFetchFailed(Exception exception)
        {
            lock (updateLock)
            {
                isRunning = false;
            }
        }
    }
}