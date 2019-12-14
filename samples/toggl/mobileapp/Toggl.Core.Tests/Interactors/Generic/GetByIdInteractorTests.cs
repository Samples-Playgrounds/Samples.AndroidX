using FluentAssertions;
using NSubstitute;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Tests.Mocks;
using Xunit;

namespace Toggl.Core.Tests.Interactors.Generic
{
    public class GetByIdInteractorTests
    {
        public sealed class TheGetIdInteractor : BaseInteractorTests
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsTheEntityWithTheGivenId()
            {
                const long id = 42;
                var entity = new MockTimeEntry();
                DataSource.TimeEntries.GetById(id).Returns(Observable.Return(entity));

                var result = await InteractorFactory.GetTimeEntryById(id).Execute();

                result.Should().BeSameAs(entity);
            }
        }
    }
}
