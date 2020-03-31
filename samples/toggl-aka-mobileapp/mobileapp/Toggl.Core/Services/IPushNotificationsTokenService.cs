using Toggl.Shared;

namespace Toggl.Core.Services
{
    public interface IPushNotificationsTokenService
    {
        PushNotificationsToken? Token { get; }
        void InvalidateCurrentToken();
    }
}
