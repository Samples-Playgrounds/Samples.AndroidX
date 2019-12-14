using FluentAssertions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.Network;
using Toggl.Networking.Tests.Integration.BaseTests;
using Xunit;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class LocationApiTests
    {
        public sealed class TheGetMethod : EndpointTestBase
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsNonEmptyCountry()
            {
                var api = TogglApiWith(Credentials.None);

                var location = await api.Location.Get();

                location.CountryName.Should().NotBeNullOrEmpty();
                location.CountryCode.Should().NotBeNullOrEmpty();
            }
        }
    }
}
