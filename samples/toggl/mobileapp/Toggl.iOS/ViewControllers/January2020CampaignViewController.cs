using CoreAnimation;
using CoreGraphics;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public sealed partial class January2020CampaignViewController
        : ReactiveViewController<January2020CampaignViewModel>
    {
        public January2020CampaignViewController(January2020CampaignViewModel viewModel)
            : base(viewModel, nameof(January2020CampaignViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            prepareViews();
            PreferredContentSize = new CGSize(288, 410);

            TitleLabel.Text = Resources.January2020CampaignTitle;
            LearnMoreButton.SetTitle(Resources.January2020CampaignPositiveButtonText, UIControlState.Normal);
            DismissButton.SetTitle(Resources.January2020CampaignNegativeButtonText, UIControlState.Normal);
            TextLabel.Text = ViewModel.CampaignMessage;

            DismissButton.Rx()
                .BindAction(ViewModel.Dismiss)
                .DisposedBy(DisposeBag);

            LearnMoreButton.Rx()
                .BindAction(ViewModel.OpenBrowser)
                .DisposedBy(DisposeBag);
        }

        private void prepareViews()
        {
            var gradient = new CAGradientLayer();
            gradient.Frame = BlueRopeView.Bounds;
            gradient.Colors = new[]
            {
                new Color("#9dd7f8").ToNativeColor().CGColor,
                new Color("#77a9e5").ToNativeColor().CGColor
            };

            BlueRopeView.Layer.InsertSublayer(gradient, 0);
        }
    }
}

