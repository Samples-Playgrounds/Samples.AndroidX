using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.Exceptions;
using Toggl.Core.Tests.Generators;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    using static January2020CampaignConfiguration.AvailableOption;

    public sealed class January2020CampaignViewModelTests
    {
        public abstract class BaseJanuary2020ViewModelTest : BaseViewModelTests<January2020CampaignViewModel>
        {
            protected override January2020CampaignViewModel CreateViewModel()
                => new January2020CampaignViewModel(
                    OnboardingStorage,
                    AnalyticsService,
                    RemoteConfigService,
                    PlatformInfo,
                    SchedulerProvider,
                    NavigationService);
        }

        public sealed class TheConstructor : BaseJanuary2020ViewModelTest
        {
            [NUnit.Framework.Theory]
            [ConstructorData]
            public void ThrowsIfSomeOfTheArgumentsIsNull(
                bool useOnboardingStorage,
                bool useAnalyticsService,
                bool useRemoteConfigService,
                bool usePlatformInfo,
                bool useSchedulerProvider,
                bool useNavigationService)
            {
                var onboardingStorage = useOnboardingStorage ? OnboardingStorage : null;
                var analyticsService = useAnalyticsService ? AnalyticsService : null;
                var remoteConfigService = useRemoteConfigService ? RemoteConfigService : null;
                var platformInfo = usePlatformInfo ? PlatformInfo : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
                var navigationService = useNavigationService ? NavigationService : null;

                Action createVM = () => new January2020CampaignViewModel(
                    onboardingStorage,
                    analyticsService,
                    remoteConfigService,
                    platformInfo,
                    schedulerProvider,
                    navigationService);

                createVM.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheViewAppearedMethod : BaseJanuary2020ViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void UpdatesTheOnboardingStorage()
            {
                OnboardingStorage.WasJanuary2020CampaignShown().Returns(false);

                ViewModel.ViewAppeared();

                OnboardingStorage.Received().SetJanuary2020CampaignWasShown();
            }

            [Fact, LogIfTooSlow]
            public void TracksCorrectAnalyticsEvent()
            {
                OnboardingStorage.WasJanuary2020CampaignShown().Returns(false);

                ViewModel.ViewAppeared();

                View.DidNotReceive().Close();
                AnalyticsService.MarketingMessageShown.Received().Track(Arg.Is("january2020"));
            }

            [Fact, LogIfTooSlow]
            public void TracksTheAnalyticsEventJustOnceEvenIfItAppearsMultipleTimes()
            {
                OnboardingStorage.WasJanuary2020CampaignShown().Returns(true);

                ViewModel.ViewAppeared();

                AnalyticsService.MarketingMessageShown.DidNotReceive().Track(Arg.Any<string>());
            }
        }

        public sealed class TheDismissAction : BaseJanuary2020ViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void TracksCorrectAnalyticsEvent()
            {
                ViewModel.Dismiss.Execute();

                AnalyticsService.MarketingMessageDismissed.Received().Track(Arg.Is("january2020"));
            }

            [Fact, LogIfTooSlow]
            public void ClosesTheView()
            {
                ViewModel.Dismiss.Execute();

                View.Received().Close();
            }
        }

        public sealed class TheFormatUrlFunction
        {
            [Theory, LogIfTooSlow]
            [InlineData(Platform.Daneel, A, "https://toggl.com/winter-is-here/?utm_source=mobile-ios&utm_medium=popup&utm_campaign=winter-is-here&utm_content=copy-a")]
            [InlineData(Platform.Daneel, B, "https://toggl.com/winter-is-here/?utm_source=mobile-ios&utm_medium=popup&utm_campaign=winter-is-here&utm_content=copy-b")]
            [InlineData(Platform.Giskard, A, "https://toggl.com/winter-is-here/?utm_source=mobile-android&utm_medium=popup&utm_campaign=winter-is-here&utm_content=copy-a")]
            [InlineData(Platform.Giskard, B, "https://toggl.com/winter-is-here/?utm_source=mobile-android&utm_medium=popup&utm_campaign=winter-is-here&utm_content=copy-b")]
            public async Task OpensTheBrowserAtCorrectUrl(
                Platform platform, January2020CampaignConfiguration.AvailableOption option, string expectedUrl)
            {
                var url = January2020CampaignViewModel.FormatUrl(option, platform);

                url.Should().Be(expectedUrl);
            }

            [Theory, LogIfTooSlow]
            [InlineData(Platform.Daneel)]
            [InlineData(Platform.Giskard)]
            public async Task PreventsFormattingAURLWhenTheConfigurationOptionIsNone(Platform platform)
            {
                Action formattingUrl = () => January2020CampaignViewModel.FormatUrl(None, platform);

                formattingUrl.Should().Throw<InvalidOperationException>();
            }
        }

        public sealed class TheOpenBrowserAction : BaseJanuary2020ViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void TracksCorrectAnalyticsEvent()
            {
                ViewModel.OpenBrowser.Execute();

                AnalyticsService.MarketingMessageCallToActionHit.Received().Track(Arg.Is("january2020"));
            }
        }
    }
}
