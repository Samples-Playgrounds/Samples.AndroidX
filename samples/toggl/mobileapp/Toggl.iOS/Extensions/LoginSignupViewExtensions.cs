using CoreGraphics;
using UIKit;

namespace Toggl.iOS.Extensions
{
    public static class LoginSignupViewExtensions
    {
        public static void SetupGoogleButton(this UIButton button)
        {
            var layer = button.Layer;
            var shadowPath = UIBezierPath.FromRect(button.Bounds);
            layer.MasksToBounds = false;
            layer.ShadowColor = UIColor.Black.CGColor;
            layer.ShadowOffset = new CGSize(0, 1);
            layer.ShadowOpacity = 0.24f;
            layer.ShadowPath = shadowPath.CGPath;
            layer.ShadowRadius = 1;

            //Add spacing between button title ang google logo
            var spacing = 17;
            button.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, spacing);
            button.TitleEdgeInsets = new UIEdgeInsets(0, spacing, 0, 0);
        }

        public static void SetupBottomCard(this UIView card)
        {
            var layer = card.Layer;
            var shadowPath = UIBezierPath.FromRect(card.Bounds);
            layer.MasksToBounds = false;
            layer.ShadowColor = UIColor.Black.CGColor;
            layer.ShadowOffset = new CGSize(0, -2);
            layer.ShadowOpacity = 0.1f;
            layer.ShadowPath = shadowPath.CGPath;
            layer.ShadowRadius = 16;
        }

        public static void SetupShowPasswordButton(this UIButton button)
        {
            var image = UIImage
                .FromBundle("icPassword")
                .ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            button.SetImage(image, UIControlState.Normal);
            button.TintColor = ColorAssets.Text;
        }
    }
}
