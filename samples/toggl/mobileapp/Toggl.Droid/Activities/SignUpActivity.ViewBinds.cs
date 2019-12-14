using Android.Views;
using Android.Widget;
using Google.Android.Material.TextField;

namespace Toggl.Droid.Activities
{
    public sealed partial class SignUpActivity
    {
        private Button signupButton;

        private View loginCard;
        private View googleSignupButton;

        private ImageView countryErrorView;

        private EditText emailEditText;
        private EditText passwordEditText;

        private TextInputLayout signUpEmailLabel;
        private TextInputLayout signUpPasswordLabel;

        private TextView errorTextView;
        private TextView countryNameTextView;
        private TextView signUpOrLabel;
        private TextView signUpWithGoogleLabel;
        private TextView alreadyHaveAnAccountLabel;
        private TextView loginLabel;

        private LinearLayout countrySelection;

        private ProgressBar progressBar;

        protected override void InitializeViews()
        {
            signUpEmailLabel = FindViewById<TextInputLayout>(Resource.Id.SignUpEmail);
            signUpPasswordLabel = FindViewById<TextInputLayout>(Resource.Id.SignUpPassword);
            loginCard = FindViewById(Resource.Id.LoginSignupCardView);
            errorTextView = FindViewById<TextView>(Resource.Id.SignUpError);
            countryNameTextView = FindViewById<TextView>(Resource.Id.SignUpCountryName);
            signUpOrLabel = FindViewById<TextView>(Resource.Id.SignUpOrLabel);
            signUpWithGoogleLabel = FindViewById<TextView>(Resource.Id.SignUpWithGoogleLabel);
            alreadyHaveAnAccountLabel = FindViewById<TextView>(Resource.Id.AlreadyHaveAnAccountLabel);
            loginLabel = FindViewById<TextView>(Resource.Id.LoginLabel);
            countrySelection = FindViewById<LinearLayout>(Resource.Id.SignUpCountrySelection);
            signupButton = FindViewById<Button>(Resource.Id.SignUpButton);
            googleSignupButton = FindViewById<View>(Resource.Id.SignUpWithGoogleButton);
            progressBar = FindViewById<ProgressBar>(Resource.Id.SignUpProgressBar);
            emailEditText = FindViewById<EditText>(Resource.Id.SignUpEmailEditText);
            passwordEditText = FindViewById<EditText>(Resource.Id.SignUpPasswordEditText);
            countryErrorView = FindViewById<ImageView>(Resource.Id.SignUpCountryErrorView);
            
            signUpEmailLabel.Hint = Shared.Resources.Email;
            signUpPasswordLabel.Hint = Shared.Resources.Password;
            signUpOrLabel.Text = Shared.Resources.Or;
            signUpWithGoogleLabel.Text = Shared.Resources.GoogleSignUp;
            alreadyHaveAnAccountLabel.Text = Shared.Resources.AlreadyHaveAnAccountQuestionMark;
            loginLabel.Text = Shared.Resources.LoginTitle;
            signupButton.Text = Shared.Resources.SignUpTitle;
        }
    }
}
