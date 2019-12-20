using Android.App;
using Android.Content;
using Toggl.Core;

namespace Toggl.Droid.BroadcastReceivers
{
    [BroadcastReceiver(Enabled = false)]
    [IntentFilter(new[] { Intent.ActionTimezoneChanged })]
    public class TimezoneChangedBroadcastReceiver : BroadcastReceiver
    {
        private ITimeService timeService;

        public TimezoneChangedBroadcastReceiver()
        {

        }

        public TimezoneChangedBroadcastReceiver(ITimeService timeService)
        {
            this.timeService = timeService;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            timeService.SignificantTimeChanged();
        }
    }
}
