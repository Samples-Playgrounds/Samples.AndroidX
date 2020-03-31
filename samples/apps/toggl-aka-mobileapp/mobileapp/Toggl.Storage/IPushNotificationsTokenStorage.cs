using System;
using Toggl.Shared;
using Toggl.Storage.Settings;

namespace Toggl.Storage
{
    public interface IPushNotificationsTokenStorage
    {
        PushNotificationsToken? PreviouslyRegisteredToken { get; }
        DateTimeOffset? DateOfRegisteringTheToken { get; }

        void StoreRegisteredToken(PushNotificationsToken token, DateTimeOffset now);
        void Clear();
    }
}
