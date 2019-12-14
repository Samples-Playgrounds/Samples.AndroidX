using System.Threading.Tasks;
using Firebase.Iid;
using Toggl.Core.Services;
using Toggl.Shared;

namespace Toggl.Droid.Services
{
    public sealed class PushNotificationsTokenServiceAndroid : IPushNotificationsTokenService
    {
        public PushNotificationsToken? Token => getToken();

        public void InvalidateCurrentToken()
        {
            Task.Run(() => FirebaseInstanceId.Instance.DeleteInstanceId());
        }

        private PushNotificationsToken? getToken()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            if (string.IsNullOrEmpty(refreshedToken))
                return null;

            return new PushNotificationsToken(refreshedToken);
        }
    }
}
