using Android.Widget;

namespace Toggl.Droid.Activities
{
    public partial class AboutActivity
    {
        private TextView licensesButton;
        private TextView privacyPolicyButton;
        private TextView termsOfServiceButton;

        protected override void InitializeViews()
        {
            licensesButton = FindViewById<TextView>(Resource.Id.LicensesButton);
            privacyPolicyButton = FindViewById<TextView>(Resource.Id.PrivacyPolicyButton);
            termsOfServiceButton = FindViewById<TextView>(Resource.Id.TermsOfServiceButton);
            
            licensesButton.Text = Shared.Resources.Licenses;
            privacyPolicyButton.Text = Shared.Resources.PrivacyPolicy;
            termsOfServiceButton.Text = Shared.Resources.TermsOfService;

            SetupToolbar(title: Shared.Resources.About);
        }
    }
}
