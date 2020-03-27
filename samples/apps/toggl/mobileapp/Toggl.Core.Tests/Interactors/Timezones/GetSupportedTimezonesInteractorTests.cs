using FluentAssertions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Interactors.Timezones;
using Xunit;

namespace Toggl.Core.Tests.Interactors.Timezones
{
    public sealed class GetSupportedTimezonesInteractorTests
    {
        public sealed class TheExecuteMethod
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsTheTimezonesApiResponse()
            {
                var interactor = new GetSupportedTimezonesInteractor();

                var returnedTimezones = await interactor.Execute();

                returnedTimezones.Should().NotBeEmpty();
            }
        }
    }
}
