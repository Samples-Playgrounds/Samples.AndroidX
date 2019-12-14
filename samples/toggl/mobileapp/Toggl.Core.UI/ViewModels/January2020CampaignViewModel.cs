using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;
using Xamarin.Essentials;

namespace Toggl.Core.UI.ViewModels
{
    using static January2020CampaignConfiguration.AvailableOption;

    public sealed class January2020CampaignViewModel : ViewModel
    {
        private readonly IOnboardingStorage onboardingStorage;
        private readonly IAnalyticsService analyticsService;
        private readonly IRemoteConfigService remoteConfigService;
        private readonly IPlatformInfo platformInfo;

        private const string campaignName = "january2020";

        public ViewAction Dismiss { get; }
        public ViewAction OpenBrowser { get; }

        public string CampaignMessage
            => remoteConfigService.GetJanuary2020CampaignConfiguration().Option == A
                ? Resources.January2020CampaignTextVersionA
                : Resources.January2020CampaignTextVersionB;

        public January2020CampaignViewModel(
            IOnboardingStorage onboardingStorage,
            IAnalyticsService analyticsService,
            IRemoteConfigService remoteConfigService,
            IPlatformInfo platformInfo,
            ISchedulerProvider schedulerProvider,
            INavigationService navigationService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(remoteConfigService, nameof(remoteConfigService));
            Ensure.Argument.IsNotNull(platformInfo, nameof(platformInfo));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));

            this.onboardingStorage = onboardingStorage;
            this.analyticsService = analyticsService;
            this.remoteConfigService = remoteConfigService;
            this.platformInfo = platformInfo;

            Dismiss = ViewAction.FromAction(dismiss, schedulerProvider.MainScheduler);
            OpenBrowser = ViewAction.FromAsync(openBrowser, schedulerProvider.MainScheduler);
        }

        public static string FormatUrl(January2020CampaignConfiguration.AvailableOption option, Platform platform)
        {
            if (option == None)
                throw new InvalidOperationException("The campaign group has to be either A or B. The January 2020 Campaign cannot be shown if it is not enabled.");

            var group = option == A ? "a" : "b";
            var os = platform == Platform.Giskard ? "android" : "ios";

            return $"https://toggl.com/winter-is-here/?utm_source=mobile-{os}&utm_medium=popup&utm_campaign=winter-is-here&utm_content=copy-{group}";
        }

        public override void ViewAppeared()
        {
            // This method will be called multiple times if the user puts the app into background
            // while this VM is being shown and then goes back to the app. We must log the event
            // only once even in this case.
            //
            // We decided to put the logic into this method and not in `Initialize` because a VM
            // can be initialized, but the view might not be shown (if the initialization or
            // presentation fails for some reason).
            if (!onboardingStorage.WasJanuary2020CampaignShown())
            {
                onboardingStorage.SetJanuary2020CampaignWasShown();
                analyticsService.MarketingMessageShown.Track(campaignName);
            }
        }

        private void dismiss()
        {
            analyticsService.MarketingMessageDismissed.Track(campaignName);

            Close();
        }

        private async Task openBrowser()
        {
            analyticsService.MarketingMessageCallToActionHit.Track(campaignName);

            var url = FormatUrl(
                remoteConfigService.GetJanuary2020CampaignConfiguration().Option,
                platformInfo.Platform);
            await Browser.OpenAsync(url, BrowserLaunchMode.External);

            Close();
        }
    }
}
