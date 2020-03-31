using CoreGraphics;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Shared.Extensions
{
    public static class ColorExtensions
    {
        public static UIColor ToNativeColor(this Color color)
            => UIColor.FromRGBA(color.Red, color.Green, color.Blue, color.Alpha);
    }

    public class ColorAssets
    {
        public static readonly UIColor Background = UIColor.FromName("Background");
        public static readonly UIColor Text = UIColor.FromName("Text");
        public static readonly UIColor InverseText = UIColor.FromName("InverseText");
        public static readonly UIColor Text1 = UIColor.FromName("Text1");
        public static readonly UIColor Text2 = UIColor.FromName("Text2");
        public static readonly UIColor Text3 = UIColor.FromName("Text3");
        public static readonly UIColor Text4 = UIColor.FromName("Text4");
        public static readonly UIColor Separator = UIColor.FromName("Separator");
        public static readonly UIColor OpaqueSeparator = UIColor.FromName("OpaqueSeparator");
        public static readonly UIColor TableBackground = UIColor.FromName("TableBackground");
        public static readonly UIColor CellBackground = UIColor.FromName("CellBackground");
        public static readonly UIColor CustomGray = UIColor.FromName("CustomGray");
        public static readonly UIColor CustomGray2 = UIColor.FromName("CustomGray2");
        public static readonly UIColor CustomGray3 = UIColor.FromName("CustomGray3");
        public static readonly UIColor CustomGray4 = UIColor.FromName("CustomGray4");
        public static readonly UIColor CustomGray5 = UIColor.FromName("CustomGray5");
        public static readonly UIColor CustomGray6 = UIColor.FromName("CustomGray6");
        public static readonly UIColor Spider = UIColor.FromName("Spider");


        public static readonly UIColor LightishGreen = UIColor.FromRGB(76, 217, 100);
    }
}
