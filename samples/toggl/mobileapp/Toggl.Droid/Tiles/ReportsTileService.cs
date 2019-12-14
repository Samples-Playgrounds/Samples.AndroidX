using Android.App;
using Android.Content;
using Android.Net;
using Android.Service.QuickSettings;
using Toggl.Shared;

namespace Toggl.Droid.Tiles
{
    [Service(Name = "toggl.giskard.Tiles.ReportsTileService",
             Label = "@string/Reports",
             Icon = "@drawable/reports_dark",
             Permission = "android.permission.BIND_QUICK_SETTINGS_TILE")]
    [IntentFilter(new[] { "android.service.quicksettings.action.QS_TILE" })]
    public sealed class ReportsTileService : TileService
    {
        public override void OnClick()
        {
            base.OnClick();

            var intent = new Intent(Intent.ActionView)
                .AddFlags(ActivityFlags.NewTask)
                .SetData(Uri.Parse(ApplicationUrls.Reports.Default));

            StartActivityAndCollapse(intent);
        }
    }
}
