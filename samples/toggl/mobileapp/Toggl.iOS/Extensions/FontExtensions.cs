using CoreText;
using UIKit;

namespace Toggl.iOS.Extensions
{
    public static class FontExtensions
    {
        public static UIFont GetMonospacedDigitFont(this UIFont self)
            => UIFont.FromDescriptor(self.FontDescriptor.GetMonospacedDigitFontDescriptor(), 0);

        public static UIFontDescriptor GetMonospacedDigitFontDescriptor(this UIFontDescriptor self)
            => self.CreateWithAttributes(
                new UIFontAttributes(new UIFontFeature(
                    CTFontFeatureNumberSpacing.Selector.MonospacedNumbers
                ))
            );
    }
}
