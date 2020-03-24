using FluentAssertions;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Toggl.Networking.Tests.Integration.BaseTests
{
    public abstract class AuthenticatedGetAllEndpointBaseTests<T> : AuthenticatedGetEndpointBaseTests<List<T>>
    {
        [Fact, LogTestInfo]
        public async Task ReturnsAnEmptyListWhenThereIsNoTimeEntryOnTheServer()
        {
            var (togglClient, user) = await SetupTestUser();

            var timeEntries = await CallEndpointWith(togglClient);

            timeEntries.Should().NotBeNull();
            timeEntries.Should().BeEmpty();
        }
    }
}
