using System;
using System.Reactive;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Xunit;
using Toggl.Core.Services;
using Toggl.Core.Tests.Generators;
using Toggl.Shared;
using Toggl.Storage.Settings;
using static Toggl.Core.Services.RemoteConfigKeys;
using Assert = NUnit.Framework.Assert;

namespace Toggl.Core.Tests.Services
{
    public class UpdateRemoteConfigCacheServiceTests
    {
        public abstract class UpdateRemoteConfigCacheServiceTest
        {
            protected UpdateRemoteConfigCacheService UpdateRemoteConfigCacheService { get; set; }
            protected IFetchRemoteConfigService FetchRemoteConfigService { get; set; } = Substitute.For<IFetchRemoteConfigService>();
            protected IKeyValueStorage KeyValueStorage { get; set; } = Substitute.For<IKeyValueStorage>();
            protected ITimeService TimeService { get; set; } = Substitute.For<ITimeService>();
        }

        public sealed class TheConstructor
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsWhenTheArgumentIsNull(bool useTimeService, bool useKeyValueStorage, bool useFetchRemoteConfigService)
            {
                ITimeService timeService = useTimeService ? Substitute.For<ITimeService>() : null;
                IKeyValueStorage keyValueStorage = useKeyValueStorage ? Substitute.For<IKeyValueStorage>() : null;
                IFetchRemoteConfigService fetchRemoteConfigService = useFetchRemoteConfigService ? Substitute.For<IFetchRemoteConfigService>() : null;
                Action constructor = () => new UpdateRemoteConfigCacheService(timeService, keyValueStorage, fetchRemoteConfigService);

                constructor.Should().Throw<ArgumentNullException>();
            }
        }

        public class TheFetchAndStoreRemoteConfigDataMethod : UpdateRemoteConfigCacheServiceTest
        {
            [Fact, LogIfTooSlow]
            public void ShouldNotUpdateTheCacheWhenFetchingFails()
            {
                var mockFetchRemoteConfigService = new MockFetchRemoteConfigService(false, true);
                UpdateRemoteConfigCacheService = new UpdateRemoteConfigCacheService(TimeService, KeyValueStorage, mockFetchRemoteConfigService);

                UpdateRemoteConfigCacheService.FetchAndStoreRemoteConfigData();

                KeyValueStorage.DidNotReceive().SetInt(RatingViewDelayParameter, Arg.Any<int>());
                KeyValueStorage.DidNotReceive().SetString(RatingViewTriggerParameter, Arg.Any<string>());
                KeyValueStorage.DidNotReceive().SetBool(RegisterPushNotificationsTokenWithServerParameter, Arg.Any<bool>());
                KeyValueStorage.DidNotReceive().SetBool(HandlePushNotificationsParameter, Arg.Any<bool>());
                KeyValueStorage.DidNotReceive().SetString(January2020CampaignOption, Arg.Any<string>());
            }

            [Theory, LogIfTooSlow]
            [InlineData(1, RatingViewCriterion.Continue, true, false, "A")]
            [InlineData(4, RatingViewCriterion.Start, false, true, "B")]
            [InlineData(5, RatingViewCriterion.None, true, true, "None")]
            public void ShouldUpdateTheCacheWhenFetchingSucceeds(int ratingViewDayCount, RatingViewCriterion ratingViewCriterion, bool registerPushes, bool handlePushes, string campaignOption)
            {
                var expectedRatingViewConfiguration = new RatingViewConfiguration(ratingViewDayCount, ratingViewCriterion);
                var expectedPushNotificationsConfiguration = new PushNotificationsConfiguration(registerPushes, handlePushes);
                var expectedCampaignConfiguration = new January2020CampaignConfiguration(campaignOption);
                var mockFetchRemoteConfigService = new MockFetchRemoteConfigService(
                    true,
                    ratingViewConfigurationToReturn: expectedRatingViewConfiguration,
                    pushNotificationsConfigurationReturn: expectedPushNotificationsConfiguration,
                    january2020CampaignConfiguration: expectedCampaignConfiguration);
                UpdateRemoteConfigCacheService = new UpdateRemoteConfigCacheService(TimeService, KeyValueStorage, mockFetchRemoteConfigService);

                UpdateRemoteConfigCacheService.FetchAndStoreRemoteConfigData();

                KeyValueStorage.Received().SetInt(RatingViewDelayParameter, ratingViewDayCount);
                KeyValueStorage.Received().SetString(RatingViewTriggerParameter, ratingViewCriterion.ToString());
                KeyValueStorage.Received().SetBool(RegisterPushNotificationsTokenWithServerParameter, registerPushes);
                KeyValueStorage.Received().SetBool(HandlePushNotificationsParameter, handlePushes);
                KeyValueStorage.Received().SetString(January2020CampaignOption, campaignOption);
            }
        }

        public class TheRemoteConfigChangedObservable : UpdateRemoteConfigCacheServiceTest
        {
            [Fact, LogIfTooSlow]
            public void ShouldEmitWhenSubscribedTo()
            {
                var testScheduler = new TestScheduler();
                var observer = testScheduler.CreateObserver<Unit>();
                UpdateRemoteConfigCacheService = new UpdateRemoteConfigCacheService(TimeService, KeyValueStorage, FetchRemoteConfigService);

                UpdateRemoteConfigCacheService.RemoteConfigChanged
                    .Subscribe(observer);

                testScheduler.Start();
                observer.Messages.Should().HaveCount(1);
            }

            [Fact, LogIfTooSlow]
            public void ShouldEmitWhenFetchAndStoreRemoteConfigDataSucceeds()
            {
                var testScheduler = new TestScheduler();
                var observer = testScheduler.CreateObserver<Unit>();
                var mockFetchRemoteConfigService = new MockFetchRemoteConfigService(shouldSucceed: true);
                UpdateRemoteConfigCacheService = new UpdateRemoteConfigCacheService(TimeService, KeyValueStorage, mockFetchRemoteConfigService);
                UpdateRemoteConfigCacheService.RemoteConfigChanged.Subscribe(observer);

                UpdateRemoteConfigCacheService.FetchAndStoreRemoteConfigData();

                testScheduler.Start();
                observer.Messages.Should().HaveCount(2);
            }

            [Fact, LogIfTooSlow]
            public void ShouldNotEmitWhenFetchAndStoreRemoteConfigDataFails()
            {
                var testScheduler = new TestScheduler();
                var observer = testScheduler.CreateObserver<Unit>();
                var mockFetchRemoteConfigService = new MockFetchRemoteConfigService(shouldSucceed: false, shouldFail: true);
                UpdateRemoteConfigCacheService = new UpdateRemoteConfigCacheService(TimeService, KeyValueStorage, mockFetchRemoteConfigService);
                UpdateRemoteConfigCacheService.RemoteConfigChanged.Subscribe(observer);
                testScheduler.Start();
                observer.Messages.Clear();

                UpdateRemoteConfigCacheService.FetchAndStoreRemoteConfigData();

                testScheduler.Start();
                observer.Messages.Should().BeEmpty();
            }
        }

        public class TheTimeSpanSinceLastFetchMethod : UpdateRemoteConfigCacheServiceTest
        {
            [Fact, LogIfTooSlow]
            public void ShouldBeTrueWhenRemoteConfigHasNeverBeenFetched()
            {
                DateTimeOffset? nullDateTimeOffset = null;
                KeyValueStorage.GetDateTimeOffset(LastFetchAtKey).Returns(nullDateTimeOffset);
                UpdateRemoteConfigCacheService = new UpdateRemoteConfigCacheService(TimeService, KeyValueStorage, FetchRemoteConfigService);

                var needsToUpdateStoredRemoteConfigData = UpdateRemoteConfigCacheService.NeedsToUpdateStoredRemoteConfigData();

                Assert.IsTrue(needsToUpdateStoredRemoteConfigData);
            }

            [Fact, LogIfTooSlow]
            public void ShouldBeTrueWhenRemoteConfigHasBeenFetchedMoreThan12HoursAndHalfAgo()
            {
                var now = new DateTimeOffset(2019, 1, 1, 12, 0, 0, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                var thirteenHoursAgo = now.AddHours(-13);
                KeyValueStorage.GetDateTimeOffset(LastFetchAtKey).Returns(thirteenHoursAgo);
                UpdateRemoteConfigCacheService = new UpdateRemoteConfigCacheService(TimeService, KeyValueStorage, FetchRemoteConfigService);

                var needsToUpdateStoredRemoteConfigData = UpdateRemoteConfigCacheService.NeedsToUpdateStoredRemoteConfigData();

                Assert.IsTrue(needsToUpdateStoredRemoteConfigData);
            }

            [Fact, LogIfTooSlow]
            public void ShouldBeFalseWhenRemoteConfigHasBeenFetchedLessThan12HoursAndHalfAgo()
            {
                var now = new DateTimeOffset(2019, 1, 1, 12, 0, 0, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                var thirteenHoursAgo = now.AddHours(-11);
                KeyValueStorage.GetDateTimeOffset(LastFetchAtKey).Returns(thirteenHoursAgo);
                UpdateRemoteConfigCacheService = new UpdateRemoteConfigCacheService(TimeService, KeyValueStorage, FetchRemoteConfigService);

                var needsToUpdateStoredRemoteConfigData = UpdateRemoteConfigCacheService.NeedsToUpdateStoredRemoteConfigData();

                Assert.IsFalse(needsToUpdateStoredRemoteConfigData);
            }
        }

        protected class MockFetchRemoteConfigService : IFetchRemoteConfigService
        {
            private readonly bool shouldSucceed;
            private readonly bool shouldFail;
            private readonly Exception exceptionOnFailure;
            private readonly RatingViewConfiguration ratingViewConfigurationToReturn;
            private readonly PushNotificationsConfiguration pushNotificationsConfigurationReturn;
            private readonly January2020CampaignConfiguration january2020CampaignConfiguration;

            public MockFetchRemoteConfigService(
                bool shouldSucceed = true,
                bool shouldFail = false,
                Exception exceptionOnFailure = null,
                RatingViewConfiguration ratingViewConfigurationToReturn = default,
                PushNotificationsConfiguration pushNotificationsConfigurationReturn = default,
                January2020CampaignConfiguration january2020CampaignConfiguration = default)
            {
                this.shouldSucceed = shouldSucceed;
                this.shouldFail = shouldFail;
                this.exceptionOnFailure = exceptionOnFailure;
                this.ratingViewConfigurationToReturn = ratingViewConfigurationToReturn;
                this.pushNotificationsConfigurationReturn = pushNotificationsConfigurationReturn;
                this.january2020CampaignConfiguration = january2020CampaignConfiguration;
            }

            public void FetchRemoteConfigData(Action onFetchSucceeded, Action<Exception> onFetchFailed)
            {
                if (shouldSucceed) onFetchSucceeded();
                if (shouldFail) onFetchFailed(exceptionOnFailure);
            }

            public RatingViewConfiguration ExtractRatingViewConfigurationFromRemoteConfig()
                => ratingViewConfigurationToReturn;

            public PushNotificationsConfiguration ExtractPushNotificationsConfigurationFromRemoteConfig()
                => pushNotificationsConfigurationReturn;

            public January2020CampaignConfiguration ExtractJanuary2020CampaignConfig()
                => january2020CampaignConfiguration;
        }
    }
}
