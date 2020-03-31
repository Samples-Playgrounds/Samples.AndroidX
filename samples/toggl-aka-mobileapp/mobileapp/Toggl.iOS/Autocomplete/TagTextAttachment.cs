using CoreGraphics;
using Foundation;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Autocomplete
{
    public sealed class TagTextAttachment : TokenTextAttachment
    {
        private static readonly UIColor borderColor = ColorAssets.Text4;

        public TagTextAttachment(string tagName, UIColor color, UIFont font)
        {
            Image = drawToken(tagName, color, font);
        }

        private static UIImage drawToken(string tagName, UIColor color, UIFont font)
        {
            var stringToDraw = new NSAttributedString(
                tagName, new UIStringAttributes { Font = font, ForegroundColor = color });

            var size = CalculateSize(stringToDraw);

            UIGraphics.BeginImageContextWithOptions(size, false, 0.0f);
            using (var context = UIGraphics.GetCurrentContext())
            {
                var tokenPath = CalculateTokenPath(size);
                context.AddPath(tokenPath.CGPath);
                context.SetStrokeColor(borderColor.CGColor);
                context.StrokePath();

                var textOffset = (TokenHeight - font.LineHeight) / 2;
                stringToDraw.DrawString(new CGPoint(TokenMargin + TokenPadding, textOffset));

                var image = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
                return image;
            }
        }
    }
}
