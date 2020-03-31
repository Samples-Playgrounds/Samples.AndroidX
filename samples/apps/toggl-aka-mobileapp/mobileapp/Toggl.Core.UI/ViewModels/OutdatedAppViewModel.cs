using System.Threading.Tasks;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xamarin.Essentials;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class OutdatedAppViewModel : ViewModel
    {
        private const string togglWebsiteUrl = "https://toggl.com";

        private readonly string storeUrl;

        public ViewAction OpenWebsite { get; }
        public ViewAction UpdateApp { get; }

        public OutdatedAppViewModel(IPlatformInfo platformInfo, IRxActionFactory rxActionFactory, INavigationService navigationService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(platformInfo, nameof(platformInfo));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            storeUrl = platformInfo.StoreUrl;

            UpdateApp = rxActionFactory.FromAsync(updateApp);
            OpenWebsite = rxActionFactory.FromAsync(openWebsite);
        }

        private Task openWebsite()
         => Browser.OpenAsync(togglWebsiteUrl, BrowserLaunchMode.External);

        private Task updateApp()
            => Browser.OpenAsync(storeUrl, BrowserLaunchMode.External);
    }
}
