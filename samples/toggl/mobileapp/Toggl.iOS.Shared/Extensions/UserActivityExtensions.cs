using Foundation;

namespace Toggl.iOS.Shared.Extensions
{
    public static class UserActivityExtensions
    {
        private const string entryDescriptionKey = "entryDescriptionKey";
        private const string responseTextKey = "responseTextKey";

        public static void SetEntryDescription(this NSUserActivity userActivity, string entryDescription)
            => userActivity.AddUserInfoEntries(new NSDictionary(entryDescriptionKey, entryDescription));

        public static NSString GetEntryDescription(this NSUserActivity userActivity)
           => (NSString)userActivity.UserInfo[entryDescriptionKey];

        public static void SetResponseText(this NSUserActivity userActivity, string responseText)
            => userActivity.AddUserInfoEntries(new NSDictionary(responseTextKey, responseText));

        public static NSString GetResponseText(this NSUserActivity userActivity)
           => (NSString)userActivity.UserInfo[responseTextKey];
    }
}
