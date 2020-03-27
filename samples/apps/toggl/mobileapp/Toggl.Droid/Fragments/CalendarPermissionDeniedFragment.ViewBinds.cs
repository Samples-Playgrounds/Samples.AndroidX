using Android.Views;
using Android.Widget;

namespace Toggl.Droid.Fragments
{
    public sealed partial class CalendarPermissionDeniedFragment
    {
        private TextView titleView;
        private TextView messageView;
        private Button continueButton;
        private Button allowAccessButton;

        protected override void InitializeViews(View view)
        {
            titleView = view.FindViewById<TextView>(Resource.Id.Title);
            messageView = view.FindViewById<TextView>(Resource.Id.Message);
            continueButton = view.FindViewById<Button>(Resource.Id.Continue);
            allowAccessButton = view.FindViewById<Button>(Resource.Id.AllowAccess);
            
            titleView.Text = Shared.Resources.NoWorries;
            messageView.Text = Shared.Resources.CalendarAccessExplanation;
            continueButton.Text = Shared.Resources.Continue;
            allowAccessButton.Text = Shared.Resources.AllowAccess;
        }
    }
}
