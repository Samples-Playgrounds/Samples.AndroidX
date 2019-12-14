using AdjustBindingsiOS;

namespace Toggl.iOS
{
    public partial class AppDelegate
    {
        private void initializeAnalytics()
        {
#if USE_APPCENTER
            Microsoft.AppCenter.AppCenter.Start(
                "{TOGGL_APP_CENTER_ID_IOS}",
                typeof(Microsoft.AppCenter.Crashes.Crashes),
                typeof(Microsoft.AppCenter.Analytics.Analytics));
#endif
#if USE_ANALYTICS
            Google.SignIn.SignIn.SharedInstance.ClientID =
                Firebase.Core.App.DefaultInstance.Options.ClientId;
            Adjust.AppDidLaunch(ADJConfig.ConfigWithAppToken("{TOGGL_ADJUST_APP_TOKEN}", AdjustConfig.EnvironmentProduction));
#endif
        }
    }
}
