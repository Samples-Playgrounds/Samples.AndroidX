using System.Threading.Tasks;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xamarin.Essentials;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class TermsOfServiceViewModel : ViewModelWithOutput<bool>
    {
        private const string privacyPolicyUrl = "https://toggl.com/legal/privacy/";
        private const string termsOfServiceUrl = "https://toggl.com/legal/terms/";

        public int IndexOfPrivacyPolicy { get; }
        public int IndexOfTermsOfService { get; }
        public string FormattedDialogText { get; }
        public ViewAction ViewTermsOfService { get; }
        public ViewAction ViewPrivacyPolicy { get; }

        public TermsOfServiceViewModel(IRxActionFactory rxActionFactory, INavigationService navigationService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            ViewPrivacyPolicy = rxActionFactory.FromAsync(openPrivacyPolicy);
            ViewTermsOfService = rxActionFactory.FromAsync(openTermsOfService);
            FormattedDialogText = string.Format(
                Resources.TermsOfServiceDialogMessage,
                Resources.TermsOfService,
                Resources.PrivacyPolicy);

            IndexOfPrivacyPolicy = FormattedDialogText.IndexOf(Resources.PrivacyPolicy);
            IndexOfTermsOfService = FormattedDialogText.IndexOf(Resources.TermsOfService);
        }

        private Task openPrivacyPolicy()
            => Browser.OpenAsync(privacyPolicyUrl, BrowserLaunchMode.External);

        private Task openTermsOfService()
            => Browser.OpenAsync(termsOfServiceUrl, BrowserLaunchMode.External);
    }
}
