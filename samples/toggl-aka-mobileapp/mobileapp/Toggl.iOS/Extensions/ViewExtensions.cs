using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Toggl.iOS.Extensions
{
    public static class ViewExtensions
    {
        public static void Shake(this UIView view, double duration = 0.05, int repeatCount = 5, double shakeThreshold = 4.0)
        {
            var animation = CABasicAnimation.FromKeyPath("position");
            animation.Duration = duration;
            animation.RepeatCount = repeatCount;
            animation.AutoReverses = true;
            animation.From =
                NSValue.FromCGPoint(new CGPoint(view.Center.X - shakeThreshold, view.Center.Y));

            animation.To = NSValue.FromCGPoint(new CGPoint(view.Center.X + shakeThreshold, view.Center.Y));
            view.Layer.AddAnimation(animation, "shake");
        }

        public static void ConstrainInView(this UIView self, UIView view)
        {
            self.TopAnchor.ConstraintEqualTo(view.TopAnchor).Active = true;
            self.BottomAnchor.ConstraintEqualTo(view.BottomAnchor).Active = true;
            self.LeadingAnchor.ConstraintEqualTo(view.LeadingAnchor).Active = true;
            self.TrailingAnchor.ConstraintEqualTo(view.TrailingAnchor).Active = true;
        }

        public static void ConstrainToViewSides(this UIView self, UIView view)
        {
            self.LeadingAnchor.ConstraintEqualTo(view.LeadingAnchor).Active = true;
            self.TrailingAnchor.ConstraintEqualTo(view.TrailingAnchor).Active = true;
        }

        public static UIView GetFirstResponder(this UIView self)
        {
            if (self.IsFirstResponder)
            {
                return self;
            }

            foreach (var subview in self.Subviews)
            {
                var firstResponder = subview.GetFirstResponder();
                if (firstResponder != null)
                {
                    return firstResponder;
                }
            }

            return null;
        }

        public static UIView InsertSeparator(this UIView self, UIRectEdge edge = UIRectEdge.Bottom)
        {
            var thickness = 1 / UIScreen.MainScreen.Scale;
            var separator = new UIView();
            separator.BackgroundColor = ColorAssets.Separator;
            self.AddSubview(separator);

            separator.TranslatesAutoresizingMaskIntoConstraints = false;
            switch (edge)
            {
                case UIRectEdge.Top:
                    separator.TopAnchor.ConstraintEqualTo(self.TopAnchor).Active = true;
                    separator.LeftAnchor.ConstraintEqualTo(self.LeftAnchor).Active = true;
                    separator.RightAnchor.ConstraintEqualTo(self.RightAnchor).Active = true;
                    separator.HeightAnchor.ConstraintEqualTo(thickness).Active = true;
                    break;
                case UIRectEdge.Bottom:
                    separator.BottomAnchor.ConstraintEqualTo(self.BottomAnchor, -thickness).Active = true;
                    separator.LeftAnchor.ConstraintEqualTo(self.LeftAnchor).Active = true;
                    separator.RightAnchor.ConstraintEqualTo(self.RightAnchor).Active = true;
                    separator.HeightAnchor.ConstraintEqualTo(thickness).Active = true;
                    break;
                case UIRectEdge.Left:
                    separator.LeftAnchor.ConstraintEqualTo(self.LeftAnchor).Active = true;
                    separator.BottomAnchor.ConstraintEqualTo(self.BottomAnchor).Active = true;
                    separator.TopAnchor.ConstraintEqualTo(self.TopAnchor).Active = true;
                    separator.WidthAnchor.ConstraintEqualTo(thickness).Active = true;
                    break;
                case UIRectEdge.Right:
                    separator.RightAnchor.ConstraintEqualTo(self.RightAnchor).Active = true;
                    separator.BottomAnchor.ConstraintEqualTo(self.BottomAnchor).Active = true;
                    separator.TopAnchor.ConstraintEqualTo(self.TopAnchor).Active = true;
                    separator.WidthAnchor.ConstraintEqualTo(thickness).Active = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(edge), edge, null);
            }

            return separator;
        }
    }
}
