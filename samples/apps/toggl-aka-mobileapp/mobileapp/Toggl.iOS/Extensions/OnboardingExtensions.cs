using CoreFoundation;
using CoreGraphics;
using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Helper;
using Toggl.Storage.Extensions;
using Toggl.Storage.Onboarding;
using Toggl.Storage.Settings;
using UIKit;

namespace Toggl.iOS.Extensions
{
    public static class Onboarding
    {
        private static readonly nfloat cellRadius = 8;

        private static readonly CGSize shadowOffset = new CGSize(0, 2);

        private static readonly CGColor shadowColor = UIColor.Black.CGColor;

        private static readonly nfloat shadowRadius = 6;

        private const float shadowOpacity = 0.1f;

        private const float closeImageSize = 6;

        private const float closeImageDistanceFromEdge = 9;

        public static void MockSuggestion(this UIView view)
        {
            view.Layer.CornerRadius = cellRadius;
            view.Layer.ShadowOffset = shadowOffset;
            view.Layer.ShadowColor = shadowColor;
            view.Layer.ShadowOpacity = shadowOpacity;
            view.Layer.ShadowRadius = shadowRadius;
        }

        public static IDisposable ManageVisibilityOf(this IOnboardingStep step, UIView view)
        {
            view.Hidden = true;

            void toggleVisibilityOnMainThread(bool shouldBeVisible)
            {
                DispatchQueue.MainQueue.DispatchAsync(
                    () => toggleVisibility(shouldBeVisible));
            }

            void toggleVisibility(bool shouldBeVisible)
            {
                if (view == null) return;

                var isVisible = view.Hidden == false;
                if (isVisible == shouldBeVisible) return;

                if (view.Superview == null) return;

                if (shouldBeVisible)
                {
                    view.Hidden = false;
                    view.Alpha = 0;
                    view.Transform = CGAffineTransform.MakeScale(0.01f, 0.01f);
                    AnimationExtensions.Animate(
                        Animation.Timings.LeaveTiming,
                        Animation.Curves.Bounce,
                        () =>
                        {
                            view.Alpha = 1;
                            view.Transform = CGAffineTransform.MakeScale(1f, 1f);
                        });
                }
                else
                {
                    view.Alpha = 1;
                    view.Transform = CGAffineTransform.MakeScale(1f, 1f);
                    AnimationExtensions.Animate(
                        Animation.Timings.LeaveTiming,
                        Animation.Curves.Bounce,
                        () =>
                        {
                            view.Alpha = 0;
                            view.Transform = CGAffineTransform.MakeScale(0.01f, 0.01f);
                        },
                        () =>
                        {
                            view.Hidden = true;
                        });
                }
            }

            return step.ShouldBeVisible.Subscribe(toggleVisibilityOnMainThread);
        }

        public static void DismissByTapping(this IDismissable step, UIView view)
        {
            var tapGestureRecognizer = new UITapGestureRecognizer(() => step.Dismiss());
            view.AddGestureRecognizer(tapGestureRecognizer);

            addTooltipCloseIcon(view);
        }

        public static IDisposable ManageDismissableTooltip(this IOnboardingStep step, UIView tooltip, IOnboardingStorage storage)
        {
            var dismissableStep = step.ToDismissable(step.GetType().FullName, storage);
            dismissableStep.DismissByTapping(tooltip);
            return dismissableStep.ManageVisibilityOf(tooltip);
        }

        private static void addTooltipCloseIcon(this UIView tooltip)
        {
            var closeImage = UIImage.FromBundle("icClose").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            var imageView = new UIImageView(closeImage);
            imageView.TintColor = UIColor.White;
            imageView.TranslatesAutoresizingMaskIntoConstraints = false;

            tooltip.AddSubview(imageView);

            imageView.WidthAnchor.ConstraintEqualTo(closeImageSize).Active = true;
            imageView.HeightAnchor.ConstraintEqualTo(closeImageSize).Active = true;
            imageView.TrailingAnchor.ConstraintEqualTo(tooltip.TrailingAnchor, -closeImageDistanceFromEdge).Active = true;
            imageView.TopAnchor.ConstraintEqualTo(tooltip.TopAnchor, closeImageDistanceFromEdge).Active = true;
        }
    }
}
