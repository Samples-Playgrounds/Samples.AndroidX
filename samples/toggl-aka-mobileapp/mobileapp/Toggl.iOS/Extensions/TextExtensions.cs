using CoreGraphics;
using Foundation;
using Toggl.Core.UI.Helper;
using UIKit;

namespace Toggl.iOS.Extensions
{
    public static class TextExtensions
    {
        private const string addIcon = "icAdd";

        public static NSAttributedString PrependWithAddIcon(this string self, double fontHeight)
        {
            var textColor = Colors.StartTimeEntry.Placeholder.ToNativeColor();
            var addIconColor = Colors.StartTimeEntry.AddIconColor.ToNativeColor();

            var emptyTextString = new NSMutableAttributedString($" {self}");
            emptyTextString.AddAttribute(UIStringAttributeKey.ForegroundColor, textColor, new NSRange(0, emptyTextString.Length));

            var result = addIcon.GetAttachmentString(fontHeight);
            // We need a space before the attachment for colors to work. The Zero-width space character does not work.
            // The space makes the left margin too big. This is here so the space does not appear.
            result.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(0), new NSRange(0, 1));
            result.AddAttribute(UIStringAttributeKey.ForegroundColor, addIconColor, new NSRange(0, result.Length));
            result.Append(emptyTextString);

            return result;
        }

        public static NSAttributedString EndingWithTick(this string self, double fontHeight)
        {
            var tick = GetAttachmentString("icDoneSmall", fontHeight);
            return endingWithIcon(self, tick);
        }

        public static NSAttributedString EndingWithRefreshIcon(this string self, double fontHeight)
        {
            var refresh = GetAttachmentString("icRefresh", fontHeight);
            return endingWithIcon(self, refresh);
        }

        private static NSAttributedString endingWithIcon(string text, NSMutableAttributedString icon)
        {
            var range = new NSRange(0, 1);
            var attributes = new UIStringAttributes { ForegroundColor = UIColor.White };
            icon.AddAttributes(attributes, range);

            var result = new NSMutableAttributedString(text);
            result.Append(new NSAttributedString(" ")); // separate the text from the icon
            result.Append(icon);

            return result;
        }

        public static NSMutableAttributedString GetAttachmentString(
            this string imageName,
            double fontCapHeight,
            UIImageRenderingMode renderingMode = UIImageRenderingMode.AlwaysTemplate)
        {
            var attachment = new NSTextAttachment
            {
                Image = UIImage
                    .FromBundle(imageName)
                    .ImageWithRenderingMode(renderingMode)
            };

            var imageSize = attachment.Image.Size;
            var y = (fontCapHeight - imageSize.Height) / 2;
            attachment.Bounds = new CGRect(0, y, imageSize.Width, imageSize.Height);

            var result = new NSMutableAttributedString("");

            if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                // For older iOS versions, there neeeds to be a space before the dot, otherwise the colors don't work
                // in iOS 13, they fixed it and this hack breaks the colors
                result.Append(new NSAttributedString(" "));
                result.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(0), new NSRange(0, 1));
            }

            var attachmentString = NSAttributedString.FromAttachment(attachment);
            result.Append(attachmentString);

            return result;
        }
    }
}
