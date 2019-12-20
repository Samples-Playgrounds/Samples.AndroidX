using UIKit;

namespace Toggl.iOS.Extensions
{
    public static class ButtonExtensions
    {
        public static void SetTemplateColor(this UIButton button, UIColor color)
        {
            button.SetImage(button.CurrentImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            button.TintColor = color;
        }
    }
}
