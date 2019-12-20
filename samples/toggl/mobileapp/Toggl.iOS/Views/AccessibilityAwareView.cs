using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Toggl.iOS.Views
{
    [Register(nameof(AccessibilityAwareView))]
    public sealed class AccessibilityAwareView : UIView
    {
        private ISubject<bool> isAccessibilityElementFocusedSubject = new Subject<bool>();

        public IObservable<bool> IsAccessibilityElementFocused
            => isAccessibilityElementFocusedSubject.AsObservable();

        public AccessibilityAwareView(IntPtr handle) : base(handle)
        {
        }

        public AccessibilityAwareView(CGRect frame) : base(frame)
        {
        }

        public AccessibilityAwareView(NSCoder coder) : base(coder)
        {
        }

        public AccessibilityAwareView(NSObjectFlag t) : base(t)
        {
        }

        public override void AccessibilityElementDidBecomeFocused()
        {
            isAccessibilityElementFocusedSubject.OnNext(true);
        }

        public override void AccessibilityElementDidLoseFocus()
        {
            isAccessibilityElementFocusedSubject.OnNext(false);
        }
    }
}
