using Android.Widget;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;

namespace Toggl.Droid.Activities
{
    public sealed partial class TokenResetActivity
    {
        private TextInputLayout tokenResetPasswordLayout;
        private EditText passwordEditText;
        private ProgressBar progressBar;
        private TextView tokenResetMessageWarning;
        private TextView tokenResetMessageEnterPasswordLabel;
        private TextView emailLabel;
        private TextView signoutLabel;
        private FloatingActionButton doneButton;

        protected override void InitializeViews()
        {
            tokenResetPasswordLayout = FindViewById<TextInputLayout>(Resource.Id.TokenResetPasswordLayout);
            passwordEditText = FindViewById<EditText>(Resource.Id.TokenResetPassword);
            progressBar = FindViewById<ProgressBar>(Resource.Id.TokenResetProgressBar);
            tokenResetMessageWarning = FindViewById<TextView>(Resource.Id.TokenResetMessageWarning);
            tokenResetMessageEnterPasswordLabel = FindViewById<TextView>(Resource.Id.TokenResetMessageEnterPasswordLabel);
            emailLabel = FindViewById<TextView>(Resource.Id.TokenResetEmailLabel);
            signoutLabel = FindViewById<TextView>(Resource.Id.TokenResetSignOutLabel);
            doneButton = FindViewById<FloatingActionButton>(Resource.Id.TokenResetDoneButton);

            tokenResetPasswordLayout.Hint = Shared.Resources.Password;
            passwordEditText.Hint = Shared.Resources.Password;
            tokenResetMessageWarning.Text = Shared.Resources.APITokenResetSuccess;
            tokenResetMessageEnterPasswordLabel.Text = Shared.Resources.TokenResetInstruction;
            signoutLabel.Text = Shared.Resources.OrSignOut;
            
            SetupToolbar(Shared.Resources.LoginTitle, showHomeAsUp: false);
        }
    }
}
