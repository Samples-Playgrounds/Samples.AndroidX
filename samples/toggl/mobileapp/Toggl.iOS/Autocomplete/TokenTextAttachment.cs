using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace Toggl.iOS.Autocomplete
{
    public abstract class TokenTextAttachment : NSTextAttachment
    {
        private const float tokenCornerRadius = 6.0f;
        private const float verticalTokenOffset = -5.5f;

        protected const int LineHeight = 24;
        protected const int TokenMargin = 3;
        protected const int TokenPadding = 6;
        protected const int TokenHeight = 22;

        public override CGRect GetAttachmentBounds(NSTextContainer textContainer,
            CGRect proposedLineFragment, CGPoint glyphPosition, nuint characterIndex)
        {
            var rect = base.GetAttachmentBounds(textContainer,
                proposedLineFragment, glyphPosition, characterIndex);

            rect.Y = verticalTokenOffset;

            return rect;
        }

        protected static CGSize CalculateSize(NSAttributedString stringToDraw, int extraElementsWidth = 0)
            => new CGSize(
                stringToDraw.Size.Width + (TokenMargin * 2) + (TokenPadding * 2) + extraElementsWidth,
                TokenHeight
            );

        protected static UIBezierPath CalculateTokenPath(CGSize size)
            => UIBezierPath.FromRoundedRect(new CGRect(
                    x: TokenMargin,
                    y: 0,
                    width: size.Width - (TokenMargin * 2),
                    height: TokenHeight
               ), tokenCornerRadius);
    }
}
