using UIKit;

namespace Toggl.iOS.Extensions
{
    public static class ScrollViewExtensions
    {
        public static bool IsAtTop(this UIScrollView scrollView)
            => scrollView.ContentOffset.Y <= 0;

        public static bool IsAtBottom(this UIScrollView scrollView)
            => scrollView.ContentOffset.Y >= scrollView.ContentSize.Height - scrollView.Frame.Height;
    }
}
