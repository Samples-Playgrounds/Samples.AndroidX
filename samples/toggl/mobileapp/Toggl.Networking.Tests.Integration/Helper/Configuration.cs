using Toggl.Networking.Network;

namespace Toggl.Networking.Tests.Integration.Helper
{
    public static class Configuration
    {
        public static UserAgent UserAgent { get; }
            = new UserAgent("MobileIntegrationTests", "CAKE_COMMIT_HASH");
    }
}
