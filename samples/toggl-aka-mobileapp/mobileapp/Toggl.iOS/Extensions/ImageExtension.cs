using CoreGraphics;
using UIKit;

namespace Toggl.iOS.Extensions
{
    public static class ImageExtension
    {
        public static UIImage ImageWithColor(UIColor color)
        {
            var size = new CGSize(1, 1);
            UIGraphics.BeginImageContextWithOptions(size, false, 0);
            var rectanglePath = UIBezierPath.FromRect(new CGRect(0, 0, size.Width, size.Height));
            color.SetFill();
            rectanglePath.Fill();
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return image;
        }
    }
}
