using Android.App;
using Android.Content;
using Toggl.Droid.Helper;
using Toggl.Droid.Services;
using Toggl.Droid.Widgets;

namespace Toggl.Droid.BroadcastReceivers
{
    [BroadcastReceiver(
        Exported = true,
        Permission = "android.permission.BIND_JOB_SERVICE",
        Name = "com.toggl.giskard.BootCompletedBroadcastReceiver")]
    [IntentFilter(new[] { Intent.ActionBootCompleted, Intent.CategoryDefault })]
    public class BootCompletedBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            ScheduleEventNotificationsService.EnqueueWork(context, intent);
            AppWidgetProviderUtils.UpdateAllInstances<TimeEntryWidget>();
            AppWidgetProviderUtils.UpdateAllInstances<SuggestionsWidget>();
        }
    }
}
