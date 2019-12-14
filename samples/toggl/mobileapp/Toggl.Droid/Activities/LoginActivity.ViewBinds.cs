using Android.Views;
using Android.Widget;
using Google.Android.Material.TextField;

namespace Toggl.Droid.Activities
{
    public sealed partial class LoginActivity
    {
        private Button loginButton;

        private TextInputLayout loginEmail;
        private TextInputLayout loginPassword;

        private View signupCard;
        private View googleLoginButton;

        private EditText emailEditText;
        private EditText passwordEditText;

        private TextView errorTextView;
        private TextView forgotPasswordView;
        private TextView googleLoginLabel;
        private TextView doNotHaveAnAccountLabel;
        private TextView orLabel;
        private TextView signUpLabel;

        private ProgressBar progressBar;

        protected override void InitializeViews()
        {
            signupCard = FindViewById(Resource.Id.LoginSignupCardView);
            errorTextView = FindViewById<TextView>(Resource.Id.LoginError);
            loginButton = FindViewById<Button>(Resource.Id.LoginLoginButton);
            forgotPasswordView = FindViewById<TextView>(Resource.Id.LoginForgotPassword);
            orLabel = FindViewById<TextView>(Resource.Id.LoginOrLabel);
            googleLoginButton = FindViewById<View>(Resource.Id.LoginGoogleLogin);
            googleLoginLabel = FindViewById<TextView>(Resource.Id.LoginGoogleLoginLabel);
            doNotHaveAnAccountLabel = FindViewById<TextView>(Resource.Id.DoNotHaveAnAccountLabel);
            signUpLabel = FindViewById<TextView>(Resource.Id.SignUpLabel);
            progressBar = FindViewById<ProgressBar>(Resource.Id.LoginProgressBar);
            loginEmail = FindViewById<TextInputLayout>(Resource.Id.LoginEmail);
            emailEditText = FindViewById<EditText>(Resource.Id.LoginEmailEditText);
            loginPassword = FindViewById<TextInputLayout>(Resource.Id.LoginPassword);
            passwordEditText = FindViewById<EditText>(Resource.Id.LoginPasswordEditText);
            
            loginEmail.Hint = Shared.Resources.Email;
            loginPassword.Hint = Shared.Resources.Password;
            forgotPasswordView.Text = Shared.Resources.LoginForgotPassword;
            googleLoginLabel.Text = Shared.Resources.GoogleLogin;
            doNotHaveAnAccountLabel.Text = Shared.Resources.DoNotHaveAnAccountWithQuestionMark;
            orLabel.Text = Shared.Resources.Or;
            signUpLabel.Text = Shared.Resources.SignUpTitle;
        }
    }
}
