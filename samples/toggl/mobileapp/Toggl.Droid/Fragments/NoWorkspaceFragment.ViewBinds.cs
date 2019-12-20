using Android.Views;
using Android.Widget;

namespace Toggl.Droid.Fragments
{
    public partial class NoWorkspaceFragment
    {
        private TextView uhOhTextView;
        private TextView errorInfoTextView;
        private ProgressBar progressBar;
        private TextView tryAgainTextView;
        private TextView createWorkspaceTextView;

        protected override void InitializeViews(View rootView)
        {
            uhOhTextView = rootView.FindViewById<TextView>(Resource.Id.UhOhTextView);
            errorInfoTextView = rootView.FindViewById<TextView>(Resource.Id.ErrorInfoTextView);
            progressBar = rootView.FindViewById<ProgressBar>(Resource.Id.ProgressBar);
            tryAgainTextView = rootView.FindViewById<TextView>(Resource.Id.TryAgainTextView);
            createWorkspaceTextView = rootView.FindViewById<TextView>(Resource.Id.CreateWorkspaceTextView);
            
            uhOhTextView.Text = Shared.Resources.UhOh;
            errorInfoTextView.Text = Shared.Resources.NoWorkspaceErrorMessage;
            tryAgainTextView.Text = Shared.Resources.NoWorkspaceTryAgain;
            createWorkspaceTextView.Text = Shared.Resources.CreateNewWorkspace;
        }
    }
}
