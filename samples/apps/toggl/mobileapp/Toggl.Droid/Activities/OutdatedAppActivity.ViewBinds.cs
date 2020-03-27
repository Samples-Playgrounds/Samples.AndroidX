using Android.Widget;

namespace Toggl.Droid.Activities
{
    public sealed partial class OutdatedAppActivity
    {
        private TextView oopsTextView;
        private TextView outdatedAppMessageView;
        private Button updateAppButton;
        private Button openWebsiteButton;

        protected override void InitializeViews()
        {
            oopsTextView = FindViewById<TextView>(Resource.Id.OopsTextView);
            outdatedAppMessageView = FindViewById<TextView>(Resource.Id.OutdatedAppMessageView);
            openWebsiteButton = FindViewById<Button>(Resource.Id.OpenWebsiteButton);
            updateAppButton = FindViewById<Button>(Resource.Id.UpdateAppButton);
            
            oopsTextView.Text = Shared.Resources.Oops;
            outdatedAppMessageView.Text = Shared.Resources.AppOutdatedMessage;
            openWebsiteButton.Text = Shared.Resources.OutdatedAppTryTogglCom;
            updateAppButton.Text = Shared.Resources.UpdateTheApp;
        }
    }
}
