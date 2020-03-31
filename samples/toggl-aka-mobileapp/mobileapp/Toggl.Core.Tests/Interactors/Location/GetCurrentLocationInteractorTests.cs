using FluentAssertions;
using NSubstitute;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Interactors.Location;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Networking;
using Toggl.Shared.Models;
using Xunit;

namespace Toggl.Core.Tests.Interactors.Location
{
    public sealed class GetCurrentLocationInteractorTests
    {
        public sealed class TheExecuteMethod
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsTheLocationApiResponse()
            {
                var location = Substitute.For<ILocation>();
                var api = Substitute.For<ITogglApi>();
                api.Location.Get().ReturnsTaskOf(location);
                var interactor = new GetCurrentLocationInteractor(api);

                var returnedLocation = await interactor.Execute();

                returnedLocation.Should().BeSameAs(location);
            }
        }
    }
}
