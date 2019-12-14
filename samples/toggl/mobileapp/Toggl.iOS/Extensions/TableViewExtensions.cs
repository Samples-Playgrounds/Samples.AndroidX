using CoreGraphics;
using System;
using UIKit;

namespace Toggl.iOS.Extensions
{
    public static class TableViewExtensions
    {
        public static void CorrectOffset(this UITableView table, CGPoint originalOffset, nfloat originalFrameHeight)
        {
            table.SetContentOffset(originalOffset, false);

            if (originalOffset.Y > 0)
            {
                var adjustedVerticalOffset = originalOffset.Y;

                var updatedContentHeight = table.ContentSize.Height;
                var updatedFrameHeight = table.Frame.Height;
                if (updatedFrameHeight >= updatedContentHeight)
                {
                    adjustedVerticalOffset = 0;
                }
                else if (originalOffset.Y + originalFrameHeight > updatedContentHeight)
                {
                    var originallyVisibleContentHeight = originalOffset.Y + originalFrameHeight;
                    adjustedVerticalOffset = originalOffset.Y + updatedContentHeight - originallyVisibleContentHeight;
                }

                if (adjustedVerticalOffset != originalOffset.Y)
                    table.SetContentOffset(new CGPoint(originalOffset.X, (nfloat)Math.Max(0, adjustedVerticalOffset)), true);
            }
        }
    }
}
