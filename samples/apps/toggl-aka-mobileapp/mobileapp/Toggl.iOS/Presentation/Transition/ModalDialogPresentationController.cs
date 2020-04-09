using CoreGraphics;
using System;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using UIKit;
using static System.Math;

namespace Toggl.iOS.Presentation.Transition
{
    public class ModalDialogPresentationController : UIPresentationController
    {
        private const double iPadMinHeightWithoutKeyboard = 360;
        private const double iPadMaxWidth = 540;
        private const double iPadTopMarginNarrow = 40;
        private const double iPadTopMarginLarge = 76;
        private const double iPadStackModalViewSpacing = 40;

        private double topiPadMargin
            => PresentingViewController.TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Compact
               || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeLeft
               || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight
                ? iPadTopMarginNarrow
                : iPadTopMarginLarge;

        private double iPadMaxHeight => UIScreen.MainScreen.Bounds.Height - 2 * topiPadMargin;

        private readonly UIView dimmingView = new UIView
        {
            BackgroundColor = Colors.ModalDialog.BackgroundOverlay.ToNativeColor(),
            Alpha = 0
        };

        public ModalDialogPresentationController(UIViewController presentedViewController, UIViewController presentingViewController)
            : base(presentedViewController, presentingViewController)
        {

        }

        public override void PresentationTransitionWillBegin()
        {
            dimmingView.Frame = ContainerView.Bounds;
            ContainerView.AddSubview(dimmingView);

            var transitionCoordinator = PresentingViewController.GetTransitionCoordinator();
            transitionCoordinator.AnimateAlongsideTransition(context => dimmingView.Alpha = 0.8f, null);
        }

        public override void DismissalTransitionWillBegin()
        {
            var transitionCoordinator = PresentingViewController.GetTransitionCoordinator();
            transitionCoordinator.AnimateAlongsideTransition(context => dimmingView.Alpha = 0.0f, null);
        }

        public override void ContainerViewWillLayoutSubviews()
        {
            PresentedView.Layer.CornerRadius = 8;
            dimmingView.Frame = ContainerView.Bounds;
            PresentedView.Frame = FrameOfPresentedViewInContainerView;
        }

        public override CGSize GetSizeForChildContentContainer(IUIContentContainer contentContainer, CGSize parentContainerSize)
        {
            if (PresentingViewController.TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular)
            {
                var preferredContentHeight = PresentedViewController.PreferredContentSize.Height != 0
                    ? PresentedViewController.PreferredContentSize.Height
                    : iPadMaxHeight;

                var height = preferredContentHeight;
                var width = Min(iPadMaxWidth, parentContainerSize.Width);

                height -= iPadStackModalViewSpacing * levelsOfModalViews();

                return new CGSize(width, height);
            }

            var preferredWidth = Min(parentContainerSize.Width, PresentedViewController.PreferredContentSize.Width);
            var maxHeight = ContainerView.Bounds.Height - ContainerView.SafeAreaInsets.Top;
            var preferredHeight = Min(maxHeight, PresentedViewController.PreferredContentSize.Height);
            return new CGSize(
                preferredWidth == 0 ? parentContainerSize.Width : preferredWidth,
                preferredHeight == 0 ? maxHeight : preferredHeight);
        }

        public override CGRect FrameOfPresentedViewInContainerView
        {
            get
            {
                var containerSize = ContainerView.Bounds.Size;
                var frame = CGRect.Empty;
                frame.Size = GetSizeForChildContentContainer(PresentedViewController, containerSize);
                frame.X = (containerSize.Width - frame.Width) / 2;
                frame.Y = (containerSize.Height - frame.Height) / 2;

                if (PresentedViewController.TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular)
                {
                    frame.Y = (containerSize.Height - frame.Size.Height) / 2 + (nfloat)iPadStackModalViewSpacing * levelsOfModalViews();
                }

                return frame;
            }
        }

        private int levelsOfModalViews()
        {
            var levels = 0;
            var topVC = PresentingViewController;
            while (topVC.PresentingViewController != null)
            {
                topVC = topVC.PresentingViewController;
                levels += 1;
            }

            return levels;
        }
    }
}
