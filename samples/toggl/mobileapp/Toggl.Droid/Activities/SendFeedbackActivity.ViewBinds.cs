using Android.Views;
using Android.Widget;

namespace Toggl.Droid.Activities
{
    public partial class SendFeedbackActivity
    {
        private View errorCard;
        private TextView oopsTextView;
        private TextView errorInfoText;
        private TextView feedbackHelperTitle;
        private EditText feedbackEditText;
        private ProgressBar progressBar;

        protected override void InitializeViews()
        {
            errorCard = FindViewById<View>(Resource.Id.ErrorCard);
            oopsTextView = FindViewById<TextView>(Resource.Id.OopsTextView);
            errorInfoText = FindViewById<TextView>(Resource.Id.ErrorInfoText);
            feedbackHelperTitle = FindViewById<TextView>(Resource.Id.FeedbackHelperTitle);
            feedbackEditText = FindViewById<EditText>(Resource.Id.FeedbackEditText);
            progressBar = FindViewById<ProgressBar>(Resource.Id.ProgressBar);
            
            oopsTextView.Text = Shared.Resources.Oops;
            feedbackHelperTitle.Text = Shared.Resources.FeedbackFieldPlaceholder;
            feedbackEditText.Hint = Shared.Resources.FeedbackHint;
            SetupToolbar(Shared.Resources.SubmitFeedback);
        }
    }
}
