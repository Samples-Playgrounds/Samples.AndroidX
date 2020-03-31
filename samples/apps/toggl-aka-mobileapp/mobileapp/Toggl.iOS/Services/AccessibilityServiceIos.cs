using UIKit;
using Foundation;
using Toggl.Core.Services;

namespace Toggl.iOS.Services
{
    public class AccessibilityServiceIos : IAccessibilityService
    {
        public void PostAnnouncement(string message)
        {
            UIAccessibility.PostNotification(
                UIAccessibilityPostNotification.Announcement,
                new NSString(message)
            );
        }
    }
}
