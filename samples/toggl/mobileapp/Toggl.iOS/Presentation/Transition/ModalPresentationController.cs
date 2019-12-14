using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using Toggl.iOS.ViewControllers;
using UIKit;
using static System.Math;

namespace Toggl.iOS.Presentation.Transition
{
    public sealed class ModalPresentationController : UIPresentationController
    {
        private readonly UIImpactFeedbackGenerator feedbackGenerator;

        private readonly nfloat backgroundAlpha = 0.8f;
        private const double returnAnimationDuration = 0.1;
        private const double impactThreshold = 0.4;
        private const double iPadMinHeightWithoutKeyboard = 360;
        private const double iPadMaxWidth = 540;
        private const double iPadTopMarginNarrow = 40;
        private const double iPadTopMarginLarge = 76;
        private const double iPadStackModalViewSpacing = 40;

        private const double keyboardMargin = 10;

        private double topiPadMargin
            => UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeLeft
               || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight
                ? iPadTopMarginNarrow
                : iPadTopMarginLarge;

        private double iPadMaxHeight => UIScreen.MainScreen.Bounds.Height - 2 * topiPadMargin;

        private CGPoint originalCenter;
        private double keyboardHeight = 0.0;

        private bool isKeyboardVisible => keyboardHeight != 0.0;

        private bool finishedPresenting = false;

        private List<NSObject> observers = new List<NSObject>();

        private readonly UIView dimmingView = new UIView
        {
            BackgroundColor = ColorAssets.CustomGray4,
            Alpha = 0
        };

        public UIView AdditionalContentView { get; }
            = new UIView();

        public ModalPresentationController(UIViewController presentedViewController, UIViewController presentingViewController)
            : base(presentedViewController, presentingViewController)
        {
            var recognizer = new UITapGestureRecognizer(() => dismiss());
            AdditionalContentView.AddGestureRecognizer(recognizer);

            var dismissBySwipingDown = new UIPanGestureRecognizer(handleSwipe);
            presentedViewController.View.AddGestureRecognizer(dismissBySwipingDown);

            feedbackGenerator = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Light);

            observers.AddRange(new[]
            {
                NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillChangeStatusBarFrameNotification, onStatusBarFrameChanged),
                NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification),
                NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification)
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            observers.ForEach(NSNotificationCenter.DefaultCenter.RemoveObserver);
            observers.Clear();
        }

        public override void PresentationTransitionWillBegin()
        {
            dimmingView.Frame = ContainerView.Bounds;
            AdditionalContentView.Frame = ContainerView.Bounds;
            AdditionalContentView.Layer.ZPosition += 1;

            ContainerView.AddSubview(dimmingView);
            ContainerView.AddSubview(AdditionalContentView);

            var coordinator = PresentedViewController.GetTransitionCoordinator();
            if (coordinator == null)
            {
                dimmingView.Alpha = backgroundAlpha;
                return;
            }

            coordinator.AnimateAlongsideTransition(_ => dimmingView.Alpha = backgroundAlpha, null);
        }

        public override void DismissalTransitionWillBegin()
        {
            var coordinator = PresentedViewController.GetTransitionCoordinator();
            if (coordinator == null)
            {
                dimmingView.Alpha = 0.0f;
                return;
            }

            coordinator.AnimateAlongsideTransition(_ => dimmingView.Alpha = 0.0f, null);
        }

        public override void DismissalTransitionDidEnd(bool completed)
        {
            base.DismissalTransitionDidEnd(completed);

            if (completed)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(UIApplication.WillChangeStatusBarFrameNotification);
            }
        }

        public override void PresentationTransitionDidEnd(bool completed)
        {
            base.PresentationTransitionDidEnd(completed);
            finishedPresenting = completed;
        }

        public override void ContainerViewWillLayoutSubviews()
        {
            dimmingView.Frame = ContainerView?.Bounds ?? CGRect.Empty;
            PresentedView.Frame = FrameOfPresentedViewInContainerView;

            PresentedViewController.View.Layer.CornerRadius = 8.0f;
            PresentedViewController.View.Layer.MasksToBounds = false;
        }

        public override CGSize GetSizeForChildContentContainer(IUIContentContainer contentContainer, CGSize parentContainerSize)
        {
            if (TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular)
            {
                var preferredContentHeight = PresentedViewController.PreferredContentSize.Height != 0
                    ? PresentedViewController.PreferredContentSize.Height
                    : iPadMaxHeight;

                var height = preferredContentHeight;
                var width = Min(iPadMaxWidth, parentContainerSize.Width);
                var stackingDepth = iPadStackModalViewSpacing * levelsOfModalViews();

                height = Min(height, iPadMaxHeight - stackingDepth);
                return new CGSize(width, height);
            }

            var maxHeight = ContainerView.Bounds.Height - ContainerView.SafeAreaInsets.Top;
            var preferredHeight = Min(maxHeight, PresentedViewController.PreferredContentSize.Height);
            return new CGSize(parentContainerSize.Width, preferredHeight == 0 ? maxHeight : preferredHeight);
        }

        public override CGRect FrameOfPresentedViewInContainerView
        {
            get
            {
                if (ContainerView == null)
                    return CGRect.Empty;

                var containerSize = ContainerView.Bounds.Size;
                var frame = CGRect.Empty;
                frame.Size = GetSizeForChildContentContainer(PresentedViewController, containerSize);

                if (TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular)
                {
                    frame.X = (containerSize.Width - frame.Size.Width) / 2;
                    frame.Y = (containerSize.Height - frame.Size.Height) / 2 + (nfloat)iPadStackModalViewSpacing * levelsOfModalViews();

                    if (isKeyboardVisible)
                    {
                        frame.Y = (nfloat)topiPadMargin + (nfloat)iPadStackModalViewSpacing * levelsOfModalViews();
                    }
                }
                else
                {
                    frame.X = 0;
                    frame.Y = containerSize.Height - frame.Size.Height;

                    if (isKeyboardVisible)
                    {
                        UIView firstResponder = PresentedView.GetFirstResponder();
                        if (firstResponder != null)
                        {
                            var firstResponderFrame =
                                firstResponder.ConvertRectToView(firstResponder.Frame, PresentedView);
                            var newY = containerSize.Height - keyboardHeight - firstResponderFrame.Y - firstResponderFrame.Height - keyboardMargin;
                            frame.Y = (nfloat)Max(Min(newY, frame.Y), UIApplication.SharedApplication.StatusBarFrame.Height);
                        }
                    }
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

        private async void handleSwipe(UIPanGestureRecognizer recognizer)
        {
            var translation = recognizer.TranslationInView(recognizer.View);
            var height = FrameOfPresentedViewInContainerView.Size.Height - keyboardHeight;
            var percent = (nfloat)Max(0, translation.Y / height);

            switch (recognizer.State)
            {
                case UIGestureRecognizerState.Began:
                    originalCenter = recognizer.View.Center;
                    feedbackGenerator.Prepare();
                    break;
                case UIGestureRecognizerState.Changed:
                    var center = new CGPoint(originalCenter.X, Max(originalCenter.Y, originalCenter.Y + translation.Y));
                    recognizer.View.Center = center;
                    dimmingView.Alpha = backgroundAlpha * (1 - percent);
                    break;
                case UIGestureRecognizerState.Ended:
                case UIGestureRecognizerState.Cancelled:
                    if (percent > impactThreshold && await dismiss())
                    {
                        feedbackGenerator.ImpactOccurred();
                    }
                    else
                    {
                        resetPosition(recognizer);
                    }
                    break;
            }
        }

        private async Task<bool> dismiss()
        {
            if (PresentedViewController is IReactiveViewController reactiveViewController)
            {
                return await reactiveViewController.DismissFromNavigationController();
            }

            PresentedViewController.DismissViewController(true, null);
            return true;
        }

        private void resetPosition(UIGestureRecognizer recognizer)
        {
            UIView.Animate(returnAnimationDuration, () =>
            {
                dimmingView.Alpha = backgroundAlpha;
                recognizer.View.Center = originalCenter;
            });
        }

        private void onStatusBarFrameChanged(NSNotification notification)
        {
            ContainerView?.SetNeedsLayout();
        }

        private void OnKeyboardNotification(NSNotification notification)
        {
            var visible = notification.Name == UIKeyboard.WillShowNotification;
            keyboardHeight = visible
                ? UIKeyboard.FrameEndFromNotification(notification).Height
                : 0.0;

            if (finishedPresenting && ContainerView != null && PresentedViewController.PresentedViewController == null)
            {
                var animationDuration = UIKeyboard.AnimationDurationFromNotification(notification);
                AnimationExtensions.Animate(animationDuration, Animation.Curves.StandardEase, ContainerViewWillLayoutSubviews);
            }
            else if (PresentingViewController == null)
            {
                ContainerView?.SetNeedsLayout();
            }
        }
    }
}
