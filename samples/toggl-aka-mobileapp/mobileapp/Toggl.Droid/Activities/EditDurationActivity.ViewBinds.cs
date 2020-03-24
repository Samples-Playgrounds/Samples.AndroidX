using Android.Views;
using Android.Widget;
using Toggl.Droid.Views.EditDuration;

namespace Toggl.Droid.Activities
{
    public partial class EditDurationActivity
    {
        private TextView startLabel;
        private TextView startTimeText;
        private TextView startDateText;
        private TextView stopLabel;
        private TextView stopTimeText;
        private TextView stopTimerLabel;
        private View stopDotSeparator;
        private TextView stopDateText;
        private WheelForegroundView wheelForeground;
        private WheelDurationInput wheelNumericInput;
        private TextView durationLabel;

        protected override void InitializeViews()
        {
            startLabel = FindViewById<TextView>(Resource.Id.StartLabel);
            startTimeText = FindViewById<TextView>(Resource.Id.StartTimeText);
            startDateText = FindViewById<TextView>(Resource.Id.StartDateText);
            stopLabel = FindViewById<TextView>(Resource.Id.StopLabel);
            stopTimeText = FindViewById<TextView>(Resource.Id.StopTimeText);
            stopTimerLabel = FindViewById<TextView>(Resource.Id.StopTimerLabel);
            stopDotSeparator = FindViewById<View>(Resource.Id.StopDotSeparator);
            stopDateText = FindViewById<TextView>(Resource.Id.StopDateText);
            wheelForeground = FindViewById<WheelForegroundView>(Resource.Id.WheelForeground);
            wheelNumericInput = FindViewById<WheelDurationInput>(Resource.Id.WheelDurationInput);
            durationLabel = FindViewById<TextView>(Resource.Id.DurationLabel);

            startLabel.Text = Shared.Resources.Start;
            stopLabel.Text = Shared.Resources.Stop;
            stopTimerLabel.Text = Shared.Resources.StopTimer;
            durationLabel.Text = Shared.Resources.Duration;
            
            SetupToolbar(title: Shared.Resources.StartAndStopTime);
        }
    }
}
