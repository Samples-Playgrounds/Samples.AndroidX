using Foundation;
using Toggl.Core.Shortcuts;
using UIKit;

namespace Toggl.iOS
{
    public partial class AppDelegate
    {
        public override void PerformActionForShortcutItem(UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler)
        {
            IosDependencyContainer.Instance
                .AnalyticsService
                .ApplicationShortcut
                .Track(shortcutItem.LocalizedTitle);

            var shortcutUrlKey = new NSString(nameof(ApplicationShortcut.Url));
            if (!shortcutItem.UserInfo.ContainsKey(shortcutUrlKey))
                return;

            var shortcutUrlString = shortcutItem.UserInfo[shortcutUrlKey] as NSString;
            if (shortcutUrlString == null)
                return;

            var shortcutUrl = new NSUrl(shortcutUrlString);

            handleDeeplink(shortcutUrl);
        }
    }
}
