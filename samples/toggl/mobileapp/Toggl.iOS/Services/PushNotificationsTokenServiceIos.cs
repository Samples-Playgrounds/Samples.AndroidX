using Firebase.InstanceID;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.iOS.Services
{
    public sealed class PushNotificationsTokenServiceIos : IPushNotificationsTokenService
    {
        public PushNotificationsToken? Token => getToken();

        public void InvalidateCurrentToken()
        {
#if !DEBUG
            InstanceId.SharedInstance.DeleteId(CommonFunctions.DoNothing);
#endif
        }

        private PushNotificationsToken? getToken()
        {
#if !DEBUG
            var refreshedToken = Firebase.CloudMessaging.Messaging.SharedInstance.FcmToken;

            if (string.IsNullOrEmpty(refreshedToken))
                return null;

            return new PushNotificationsToken(refreshedToken);
#else
            return null;
#endif
        }
    }
}
