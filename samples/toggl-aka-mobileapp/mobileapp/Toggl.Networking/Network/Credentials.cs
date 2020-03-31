using System;
using System.Text;
using Toggl.Shared;

namespace Toggl.Networking.Network
{
    public sealed class Credentials
    {
        internal HttpHeader Header { get; }

        private Credentials(HttpHeader header)
        {
            Header = header;
        }

        public static Credentials None => new Credentials(HttpHeader.None);

        public static Credentials WithPassword(Email email, Password password)
        {
            if (!email.IsValid)
                throw new ArgumentException($"A valid {nameof(email)} must be provided when creating credentials");
            if (!password.IsValid)
                throw new ArgumentException($"A valid {nameof(password)} must be provided when creating credentials");

            var header = authorizationHeaderWithValue($"{email}:{password}");

            return new Credentials(header);
        }

        public static Credentials WithApiToken(string apiToken)
        {
            Ensure.Argument.IsNotNullOrEmpty(apiToken, nameof(apiToken));

            var header = authorizationHeaderWithValue($"{apiToken}:api_token");

            return new Credentials(header);
        }

        public static Credentials WithGoogleToken(string googleToken)
        {
            Ensure.Argument.IsNotNull(googleToken, nameof(googleToken));

            var header = authorizationHeaderWithValue($"{googleToken}:google_access_token");

            return new Credentials(header);
        }

        private static HttpHeader authorizationHeaderWithValue(string authString)
        {
            Ensure.Argument.IsNotNull(authString, nameof(authString));

            var authStringBytes = Encoding.UTF8.GetBytes(authString);
            var authHeader = Convert.ToBase64String(authStringBytes);

            return new HttpHeader("Authorization", authHeader, HttpHeader.HeaderType.Auth);
        }
    }
}
