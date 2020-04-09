using Toggl.Networking.Network;
using Toggl.Shared;

namespace Toggl.Networking
{
    public sealed class ApiConfiguration
    {
        public UserAgent UserAgent { get; }
        public Credentials Credentials { get; }
        public ApiEnvironment Environment { get; }

        public ApiConfiguration(ApiEnvironment apiEnvironment, Credentials credentials, UserAgent userAgent)
        {
            Ensure.Argument.IsNotNull(userAgent, nameof(userAgent));
            Ensure.Argument.IsNotNull(credentials, nameof(credentials));

            UserAgent = userAgent;
            Credentials = credentials;
            Environment = apiEnvironment;
        }
    }
}
