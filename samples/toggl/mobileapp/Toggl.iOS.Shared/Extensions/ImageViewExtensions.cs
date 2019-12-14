using UIKit;

namespace Toggl.iOS.Shared.Extensions
{
    public static class ImageViewExtensions
    {
        public static void SetTemplateColor(this UIImageView imageView, UIColor color)
        {
            imageView.Image = imageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            imageView.TintColor = color;
        }
    }
}
