using Android.Views;
using Android.Widget;

namespace Toggl.Droid.Fragments
{
    public sealed partial class TermsOfServiceFragment
    {
        private TextView reviewTheTermsTextView;
        private TextView termsMessageTextView;
        private Button acceptButton;

        protected override void InitializeViews(View fragmentView)
        {
            reviewTheTermsTextView = fragmentView.FindViewById<TextView>(Resource.Id.ReviewTheTermsTextView);
            termsMessageTextView = fragmentView.FindViewById<TextView>(Resource.Id.TermsMessageTextView);
            acceptButton = fragmentView.FindViewById<Button>(Resource.Id.AcceptButton);
            
            acceptButton.Text = Shared.Resources.IAgree;
            reviewTheTermsTextView.Text = Shared.Resources.ReviewTheTerms;
        }
    }
}
