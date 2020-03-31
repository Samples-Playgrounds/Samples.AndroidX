using FluentAssertions;
using System;
using Toggl.Networking.Network;
using Xunit;
using static Toggl.Networking.ApiEnvironment;
using static Toggl.Networking.Network.Credentials;

namespace Toggl.Networking.Tests
{
    public sealed class ApiConfigurationTests
    {
        public sealed class TheConstructor
        {
            private static readonly UserAgent correctUserAgent = new UserAgent("Test", "1.0");

            [Fact, LogIfTooSlow]
            public void ThrowsWhenTheCredentialsParameterIsNull()
            {
                Action constructingWithWrongParameteres =
                    () => new ApiConfiguration(Staging, null, correctUserAgent);

                constructingWithWrongParameteres
                    .Should().Throw<ArgumentNullException>();
            }

            [Fact, LogIfTooSlow]
            public void ThrowsWhenTheUserAgentIsNull()
            {
                Action constructingWithWrongParameteres =
                    () => new ApiConfiguration(Staging, None, null);

                constructingWithWrongParameteres
                    .Should().Throw<ArgumentException>();
            }
        }
    }
}
