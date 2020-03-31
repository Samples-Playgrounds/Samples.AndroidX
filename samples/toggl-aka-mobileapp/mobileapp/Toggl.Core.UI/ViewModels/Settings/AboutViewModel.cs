using System.Threading.Tasks;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xamarin.Essentials;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class AboutViewModel : ViewModel
    {
        public ViewAction OpenPrivacyPolicyView { get; private set; }
        public ViewAction OpenTermsOfServiceView { get; private set; }
        public ViewAction OpenLicensesView { get; private set; }

        public AboutViewModel(
            IRxActionFactory rxActionFactory,
            INavigationService navigationService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            OpenLicensesView = rxActionFactory.FromAsync(openLicensesView);
            OpenPrivacyPolicyView = rxActionFactory.FromAsync(openPrivacyPolicyView);
            OpenTermsOfServiceView = rxActionFactory.FromAsync(openTermsOfServiceView);
        }

        private Task openPrivacyPolicyView()
            => Browser.OpenAsync(Resources.PrivacyPolicyUrl, BrowserLaunchMode.SystemPreferred);

        private Task openTermsOfServiceView()
            => Browser.OpenAsync(Resources.TermsOfServiceUrl, BrowserLaunchMode.SystemPreferred);

        private Task openLicensesView()
            => Navigate<LicensesViewModel>();
    }
}
