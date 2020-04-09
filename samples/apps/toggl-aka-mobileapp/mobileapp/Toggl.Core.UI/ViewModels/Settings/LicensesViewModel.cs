using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class LicensesViewModel : ViewModel
    {
        public IImmutableList<License> Licenses { get; }

        public LicensesViewModel(ILicenseProvider licenseProvider, INavigationService navigationService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(licenseProvider, nameof(licenseProvider));

            Licenses = licenseProvider.GetAppLicenses()
                .Select(keyValuePair => new License(keyValuePair.Key, keyValuePair.Value))
                .ToImmutableList();
        }
    }
}
