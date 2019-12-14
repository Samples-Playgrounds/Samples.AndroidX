using Foundation;
using Toggl.iOS.Extensions;
using Toggl.Shared;
using UIKit;
using Colors = Toggl.Core.UI.Helper.Colors;

namespace Toggl.iOS.Views.EditDuration
{
    public static class DurationFieldTextFormatter
    {
        private static readonly UIColor placeHolderColor = Colors.Common.PlaceholderText.ToNativeColor();

        public static NSAttributedString AttributedStringFor(string durationText, UIFont font)
        {
            durationText = durationText ?? "";
            var prefixLength = DurationHelper.LengthOfDurationPrefix(durationText);
            var result = new NSMutableAttributedString(durationText, font: font.GetMonospacedDigitFont(), foregroundColor: ColorAssets.Text);
            result.AddAttribute(UIStringAttributeKey.ForegroundColor, placeHolderColor, new NSRange(0, prefixLength));
            return result;
        }
    }
}
