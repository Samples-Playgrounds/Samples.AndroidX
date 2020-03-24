using CoreFoundation;
using Foundation;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Shortcuts;
using UIKit;

namespace Toggl.iOS
{
    [Preserve(AllMembers = true)]
    public sealed class ApplicationShortcutCreator : BaseApplicationShortcutCreator
    {
        private static readonly HashSet<ShortcutType> supportedShortcuts = new HashSet<ShortcutType>(new[] {
            ShortcutType.Calendar,
            ShortcutType.Reports,
            ShortcutType.StartTimeEntry,
            ShortcutType.StopTimeEntry,
            ShortcutType.ContinueLastTimeEntry
        });

        private IReadOnlyDictionary<ShortcutType, UIApplicationShortcutIcon> icons => createIcons();

        private IReadOnlyDictionary<ShortcutType, UIApplicationShortcutIcon> createIcons()
            => new Dictionary<ShortcutType, UIApplicationShortcutIcon>
            {
                { ShortcutType.Reports, UIApplicationShortcutIcon.FromType(UIApplicationShortcutIconType.Time) },
                { ShortcutType.StopTimeEntry, UIApplicationShortcutIcon.FromType(UIApplicationShortcutIconType.Pause) },
                { ShortcutType.StartTimeEntry, UIApplicationShortcutIcon.FromType(UIApplicationShortcutIconType.Play) },
                { ShortcutType.ContinueLastTimeEntry, UIApplicationShortcutIcon.FromType(UIApplicationShortcutIconType.Play) },
                { ShortcutType.Calendar, UIApplicationShortcutIcon.FromType(UIApplicationShortcutIconType.Date) }
            };

        public ApplicationShortcutCreator() : base(supportedShortcuts)
        {
        }

        protected override void ClearAllShortCuts()
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                UIApplication.SharedApplication.ShortcutItems = new UIApplicationShortcutItem[0];
            });
        }

        protected override void SetShortcuts(IEnumerable<ApplicationShortcut> shortcuts)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                UIApplication.SharedApplication.ShortcutItems = shortcuts
                    .Select(createIosShortcut)
                    .ToArray();
            });
        }

        private UIApplicationShortcutItem createIosShortcut(ApplicationShortcut shortcut)
            => new UIApplicationShortcutItem(
                shortcut.Type.ToString(),
                shortcut.Title,
                shortcut.Subtitle,
                icons[shortcut.Type],
                userInfoFor(shortcut)
            );

        private NSDictionary<NSString, NSObject> userInfoFor(ApplicationShortcut shortcut)
            => new NSDictionary<NSString, NSObject>(new NSString(nameof(ApplicationShortcut.Url)), new NSString(shortcut.Url));
    }
}
