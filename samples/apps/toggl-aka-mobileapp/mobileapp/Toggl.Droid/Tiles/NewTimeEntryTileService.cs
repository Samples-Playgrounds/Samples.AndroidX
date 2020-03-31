using Android.App;
using Android.Content;
using Android.Net;
using Android.Service.QuickSettings;
using Toggl.Shared;

namespace Toggl.Droid.Tiles
{
    [Service(Name = "toggl.giskard.Tiles.NewTimeEntryTileService",
             Label = "@string/NewTimeEntry",
             Icon = "@drawable/play",
             Permission = "android.permission.BIND_QUICK_SETTINGS_TILE")]
    [IntentFilter(new[] { "android.service.quicksettings.action.QS_TILE" })]
    public sealed class NewTimeEntryTileService : TileService
    {
        public override void OnClick()
        {
            base.OnClick();

            var intent = new Intent(Intent.ActionView)
                .AddFlags(ActivityFlags.NewTask)
                .SetData(Uri.Parse(ApplicationUrls.TimeEntry.New.Default));

            StartActivityAndCollapse(intent);
        }
    }
}
