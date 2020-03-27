using Foundation;
using UIKit;

namespace Toggl.iOS.Shared
{
    public static class UILabelExtensions
    {
        public static void SetLineSpacing(this UILabel label, float lineSpacing, UITextAlignment alignment)
        {
            var text = label.Text ?? "";
            var attributedText = new NSMutableAttributedString(text);
            var range = new NSRange(0, text.Length);

            var paragraphStyle = new NSMutableParagraphStyle();
            paragraphStyle.LineSpacing = lineSpacing;
            paragraphStyle.Alignment = alignment;

            attributedText.AddAttribute(UIStringAttributeKey.ParagraphStyle, paragraphStyle, range);

            label.AttributedText = attributedText;
        }
    }
}
