using FluentAssertions;
using System.Reactive.Linq;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Tests.Integration.BaseTests;
using Xunit;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class CountriesApiTests
    {
        public sealed class TheGetAllMethod : EndpointTestBase
        {
            [Fact, LogTestInfo]
            public async ThreadingTask ReturnsAllCountries()
            {
                var togglClient = TogglApiWith(Credentials.None);
                var countries = await togglClient.Countries.GetAll();

                countries.Should().AllBeOfType<Country>();
                countries.Should().NotBeEmpty();
            }
        }
    }
}
