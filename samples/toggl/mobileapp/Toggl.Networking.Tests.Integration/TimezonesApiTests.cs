using FluentAssertions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.Network;
using Toggl.Networking.Tests.Integration.BaseTests;
using Xunit;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class TimezonesApiTests
    {
        public sealed class TheGetMethod : EndpointTestBase
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsNonEmptyCountry()
            {
                var api = TogglApiWith(Credentials.None);

                var timezones = await api.Timezones.GetAll();

                timezones.Should().NotBeNullOrEmpty();
            }

            [Fact, LogIfTooSlow]
            public async Task ShouldMatchOurHardcodedTimezoneJSON()
            {
                var api = TogglApiWith(Credentials.None);

                var timezones = await api.Timezones.GetAll();

                timezones.Should().HaveCount(137, "Consider update the TimezonesJson in Resources.resx");
            }
        }
    }
}
