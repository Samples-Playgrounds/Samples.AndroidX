using CoreGraphics;
using Foundation;
using UIKit;

namespace Toggl.iOS.Extensions
{
    public static class TextViewExtensions
    {
        public static void RejectAutocorrect(this UITextView textView, UIResponder scratchView)
        {
            var originalText = textView.Text;
            NSRange originalRange = textView.SelectedRange;
            CGPoint originalOffset = textView.ContentOffset;

            // force iOS to accept the autocorrect suggestion by changing the focus
            // to a differnent view - the keyboard won't hide and reappear
            scratchView.BecomeFirstResponder();
            textView.BecomeFirstResponder();

            // now throw away what the autocorrect changed in the text view (if anything)
            string finalText = textView.Text;
            if (originalText != finalText)
            {
                textView.Text = originalText;
                textView.SelectedRange = originalRange;
                textView.SetContentOffset(originalOffset, false);
            }
        }
    }
}
