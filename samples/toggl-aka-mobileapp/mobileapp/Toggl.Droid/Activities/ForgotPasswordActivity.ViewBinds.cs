using Android.Widget;
using Google.Android.Material.TextField;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.Activities
{
    public partial class ForgotPasswordActivity
    {
        private TextInputLayout loginEmail;
        private EditText loginEmailEditText;
        private Button resetPasswordButton;
        private ProgressBar loadingProgressBar;

        protected override void InitializeViews()
        {
            loginEmail = FindViewById<TextInputLayout>(Resource.Id.LoginEmail);
            loginEmailEditText = FindViewById<EditText>(Resource.Id.LoginEmailEditText);
            resetPasswordButton = FindViewById<Button>(Resource.Id.ResetPasswordButton);
            loadingProgressBar = FindViewById<ProgressBar>(Resource.Id.LoadingProgressBar);

            loginEmailEditText.SetFocus();
            loginEmailEditText.SetSelection(loginEmailEditText.Text?.Length ?? 0);

            loginEmail.HelperText = Shared.Resources.PasswordResetExplanation;
            loginEmail.Hint = Shared.Resources.Email;
            resetPasswordButton.Text = Shared.Resources.GetPasswordResetLink;
            
            SetupToolbar(Shared.Resources.LoginForgotPassword);
        }
    }
}
