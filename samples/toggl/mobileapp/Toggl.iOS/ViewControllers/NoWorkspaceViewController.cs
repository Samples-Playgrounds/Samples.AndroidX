using CoreGraphics;
using Foundation;
using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public sealed partial class NoWorkspaceViewController
        : ReactiveViewController<NoWorkspaceViewModel>
    {
        private const float cardHeight = 368;

        public NoWorkspaceViewController(NoWorkspaceViewModel viewModel) : base(viewModel, nameof(NoWorkspaceViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CreateWorkspaceButton.SetTitle(Resources.CreateNewWorkspace, UIControlState.Normal);
            HeadingLabel.Text = Resources.UhOh;
            TextLabel.Text = Resources.NoWorkspaceErrorMessage;

            var tryAgainString = string.Format(Resources.NoWorkspaceOr, Resources.NoWorkspaceTryAgain);

            var rangeStart = tryAgainString.IndexOf(Resources.NoWorkspaceTryAgain, System.StringComparison.CurrentCulture);
            var rangeEnd = Resources.NoWorkspaceTryAgain.Length;
            var range = new NSRange(rangeStart, rangeEnd);

            var attributedString = new NSMutableAttributedString(
                tryAgainString,
                new UIStringAttributes { ForegroundColor = ColorAssets.Text });
            attributedString.AddAttributes(
                new UIStringAttributes { ForegroundColor = Colors.NoWorkspace.ActivityIndicator.ToNativeColor() },
                range);

            TryAgainButton.SetAttributedTitle(attributedString, UIControlState.Normal);

            var screenWidth = UIScreen.MainScreen.Bounds.Width;
            PreferredContentSize = new CGSize
            {
                // ScreenWidth - 32 for 16pt margins on both sides
                Width = screenWidth > 320 ? screenWidth - 32 : 312,
                Height = cardHeight
            };

            prepareViews();

            CreateWorkspaceButton.Rx()
                .BindAction(ViewModel.CreateWorkspaceWithDefaultName)
                .DisposedBy(DisposeBag);

            TryAgainButton.Rx()
                .BindAction(ViewModel.TryAgain)
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .Invert()
                .Subscribe(CreateWorkspaceButton.Rx().Enabled())
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .Invert()
                .Subscribe(TryAgainButton.Rx().IsVisibleWithFade())
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading.StartWith(false)
                .Subscribe(ActivityIndicatorView.Rx().IsVisibleWithFade())
                .DisposedBy(DisposeBag);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ActivityIndicatorView.StartSpinning();
        }

        private void prepareViews()
        {
            ActivityIndicatorView.IndicatorColor = Colors.NoWorkspace.ActivityIndicator.ToNativeColor();
            CreateWorkspaceButton.SetTitleColor(Colors.NoWorkspace.DisabledCreateWorkspaceButton.ToNativeColor(), UIControlState.Disabled);
        }
    }
}
