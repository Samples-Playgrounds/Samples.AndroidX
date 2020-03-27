using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.iOS.Autocomplete;
using UIKit;
using static Toggl.Shared.Extensions.EnumerableExtensions;

namespace Toggl.iOS.Transformations
{
    public sealed class TagsListToAttributedString
    {
        private readonly TokenTextAttachment elipsisTag;
        private readonly nfloat elipsisTagWidth;
        private readonly nfloat targetWidth;
        private readonly nfloat targetBoundsWidth;

        public TagsListToAttributedString(UITextView target)
        {
            elipsisTag = "...".GetTagToken();
            elipsisTagWidth = elipsisTag.Image.Size.Width;

            targetWidth = target.ContentSize.Width;
            targetBoundsWidth = target.Bounds.Width;
        }

        public NSAttributedString Convert(IEnumerable<string> tags)
        {
            var tagTokens = tags.Select(tag => tag.GetTagToken());

            nfloat totalLength = 0;
            var cumulativeLengths = tagTokens
                .Select(token => totalLength += token.Image.Size.Width)
                .ToList();

            if (totalLength <= targetWidth)
                return createAttributedString(tagTokens);

            var tagCount = cumulativeLengths
                .IndexOf(length => length + elipsisTagWidth >= targetBoundsWidth);

            if (tagCount == -1)
                return createAttributedString(new[] { elipsisTag });

            return createAttributedString(
                tagTokens.Take(tagCount)
                .ToList()
                .Append(elipsisTag)
            );
        }

        private static NSAttributedString createAttributedString(
            IEnumerable<TokenTextAttachment> tagTokens)
        {
            var tagTokenString = tagTokens.Aggregate(
                new NSMutableAttributedString(),
                (result, token) =>
                {
                    result.Append(NSAttributedString.FromAttachment(token));
                    return result;
                }
            );

            return tagTokenString;
        }
    }
}
