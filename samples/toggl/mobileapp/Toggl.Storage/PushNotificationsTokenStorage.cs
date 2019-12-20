using System;
using Toggl.Shared;
using Toggl.Storage.Settings;

namespace Toggl.Storage
{
    public sealed class PushNotificationsTokenStorage : IPushNotificationsTokenStorage
    {
        private const string previouslyRegisteredTokenKey = "PreviouslyRegisteredTokenKey";
        private const string dateOfRegisteringPreviousTokenKey = "DateOfRegisteringPreviousTokenKey";

        private readonly IKeyValueStorage keyValueStorage;

        public PushNotificationsTokenStorage(IKeyValueStorage keyValueStorage)
        {
            Ensure.Argument.IsNotNull(keyValueStorage, nameof(keyValueStorage));

            this.keyValueStorage = keyValueStorage;
        }

        public PushNotificationsToken? PreviouslyRegisteredToken
        {
            get
            {
                var token = keyValueStorage.GetString(previouslyRegisteredTokenKey);
                return string.IsNullOrEmpty(token) ? (PushNotificationsToken?)null : new PushNotificationsToken(token);
            }
        }

        public DateTimeOffset? DateOfRegisteringTheToken
            => keyValueStorage.GetDateTimeOffset(dateOfRegisteringPreviousTokenKey);

        public void StoreRegisteredToken(PushNotificationsToken token, DateTimeOffset now)
        {
            keyValueStorage.SetString(previouslyRegisteredTokenKey, token.ToString());
            keyValueStorage.SetDateTimeOffset(dateOfRegisteringPreviousTokenKey, now);
        }

        public void Clear()
        {
            keyValueStorage.Remove(previouslyRegisteredTokenKey);
            keyValueStorage.Remove(dateOfRegisteringPreviousTokenKey);
        }
    }
}
