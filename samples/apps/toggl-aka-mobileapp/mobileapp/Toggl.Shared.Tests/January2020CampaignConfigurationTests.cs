using FluentAssertions;
using Xunit;

namespace Toggl.Shared.Tests
{
    using static January2020CampaignConfiguration.AvailableOption;

    public sealed class January2020CampaignConfigurationTests
    {
        public sealed class TheConstructor
        {
            [Theory]
            [LogIfTooSlow]
            [InlineData("a")]
            [InlineData("A")]
            public void CorrectlyParsesOptionA(string a)
            {
                var config = new January2020CampaignConfiguration(a);

                config.Option.Should().Be(A);
            }

            [Theory]
            [LogIfTooSlow]
            [InlineData("b")]
            [InlineData("B")]
            public void CorrectlyParsesOptionB(string b)
            {
                var config = new January2020CampaignConfiguration(b);

                config.Option.Should().Be(B);
            }

            [Theory]
            [LogIfTooSlow]
            [InlineData("")]
            [InlineData("none")]
            [InlineData("None")]
            [InlineData("NoNe")]
            [InlineData("Some Unknown Text")]
            public void CorrectlyParsesOptionNone(string none)
            {
                var config = new January2020CampaignConfiguration(none);

                config.Option.Should().Be(None);
            }
        }

        public sealed class TheToStringMethod
        {
            [Theory]
            [LogIfTooSlow]
            [InlineData(A)]
            [InlineData(B)]
            [InlineData(None)]
            public void SerializesToParsableString(January2020CampaignConfiguration.AvailableOption option)
            {
                var config = new January2020CampaignConfiguration(option.ToString());

                config.Option.Should().Be(option);
            }
        }
    }
}
