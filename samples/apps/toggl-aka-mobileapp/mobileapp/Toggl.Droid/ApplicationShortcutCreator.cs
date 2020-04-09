using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.Net;
using Android.Runtime;
using Java.Lang;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Shortcuts;
using Toggl.Droid.Helper;

namespace Toggl.Droid
{
    public sealed class ApplicationShortcutCreator : BaseApplicationShortcutCreator
    {
        private static readonly HashSet<ShortcutType> supportedShortcuts = new HashSet<ShortcutType>(new[] {
            ShortcutType.Reports,
            ShortcutType.StartTimeEntry,
            ShortcutType.StopTimeEntry,
            ShortcutType.ContinueLastTimeEntry
        });

        private readonly Context context;

        private readonly ShortcutManager shortcutManager;

        public ApplicationShortcutCreator(Context context) : base(supportedShortcuts)
        {
            this.context = context;

            if (NougatApis.AreNotAvailable)
                return;

            var shortcutManagerType = Class.FromType(typeof(ShortcutManager));
            shortcutManager = context.GetSystemService(shortcutManagerType).JavaCast<ShortcutManager>();
        }

        protected override void ClearAllShortCuts()
        {
            if (NougatApis.AreNotAvailable)
                return;

            shortcutManager.RemoveAllDynamicShortcuts();
        }

        protected override void SetShortcuts(IEnumerable<ApplicationShortcut> shortcuts)
        {
            if (NougatApis.AreNotAvailable)
                return;

            var droidShortcuts = shortcuts
                .Select(androidShortcut)
                .ToList();
            shortcutManager.SetDynamicShortcuts(droidShortcuts);
        }

        private ShortcutInfo androidShortcut(ApplicationShortcut shortcut)
        {
            var droidShortcut =
                new ShortcutInfo.Builder(context, shortcut.Title)
                    .SetLongLabel($"{shortcut.Title} {shortcut.Subtitle}")
                    .SetShortLabel(shortcut.Title)
                    .SetIcon(getIcon(shortcut.Type))
                    .SetIntent(new Intent(Intent.ActionView).SetData(Uri.Parse(shortcut.Url)))
                    .Build();

            return droidShortcut;
        }

        private Icon getIcon(ShortcutType type)
        {
            var resourceId = 0;
            switch (type)
            {
                case ShortcutType.ContinueLastTimeEntry:
                    resourceId = Resource.Drawable.play;
                    break;

                case ShortcutType.Reports:
                    resourceId = Resource.Drawable.reports_dark;
                    break;

                case ShortcutType.StartTimeEntry:
                    resourceId = Resource.Drawable.play;
                    break;

                case ShortcutType.StopTimeEntry:
                    resourceId = Resource.Drawable.stop_white;
                    break;
            }

            var icon = Icon.CreateWithResource(context, resourceId);
            return icon;
        }
    }
}
