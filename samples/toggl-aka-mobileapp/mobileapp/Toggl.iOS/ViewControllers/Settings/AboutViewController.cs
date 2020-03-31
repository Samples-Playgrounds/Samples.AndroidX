using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.iOS.ViewControllers
{
    public sealed partial class AboutViewController : ReactiveViewController<AboutViewModel>
    {
        public AboutViewController(AboutViewModel viewModel)
            : base(viewModel, nameof(AboutViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = Resources.About;

            PrivacyPolicyLabel.Text = Resources.PrivacyPolicy;
            TermsOfServiceLabel.Text = Resources.TermsOfService;
            LicensesLabel.Text = Resources.Licenses;

            PrivacyPolicyView.InsertSeparator(UIKit.UIRectEdge.Top);
            PrivacyPolicyView.InsertSeparator();
            TermsOfServiceView.InsertSeparator();
            LicensesView.InsertSeparator();

            LicensesView.Rx()
                .BindAction(ViewModel.OpenLicensesView)
                .DisposedBy(DisposeBag);

            PrivacyPolicyView.Rx()
                .BindAction(ViewModel.OpenPrivacyPolicyView)
                .DisposedBy(DisposeBag);

            TermsOfServiceView.Rx()
                .BindAction(ViewModel.OpenTermsOfServiceView)
                .DisposedBy(DisposeBag);
        }
    }
}
