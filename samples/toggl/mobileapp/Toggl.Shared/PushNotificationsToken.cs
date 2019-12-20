using System;

namespace Toggl.Shared
{
    public struct PushNotificationsToken : IEquatable<PushNotificationsToken>
    {
        private readonly string token;

        public PushNotificationsToken(string token)
        {
            Ensure.Argument.IsNotNullOrEmpty(token, nameof(token));

            this.token = token;
        }

        public bool Equals(PushNotificationsToken other)
            => token == other.token;

        public override bool Equals(object obj)
            => obj is PushNotificationsToken pushNotificationsToken && Equals(pushNotificationsToken);

        public override int GetHashCode()
            => token.GetHashCode();

        public override string ToString() => token;

        public static bool operator ==(PushNotificationsToken a, PushNotificationsToken b)
            => a.Equals(b);

        public static bool operator !=(PushNotificationsToken a, PushNotificationsToken b)
            => !a.Equals(b);
    }
}
