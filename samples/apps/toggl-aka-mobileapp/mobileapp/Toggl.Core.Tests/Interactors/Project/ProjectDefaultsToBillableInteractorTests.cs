using FluentAssertions;
using NSubstitute;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Tests.Mocks;
using Xunit;

namespace Toggl.Core.Tests.Interactors
{
    public class ProjectDefaultsToBillableInteractorTests
    {
        public sealed class TheProjectDefaultsToBillableInteractorInteractor : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public async Task ReturnsTheBillableValueOfTheQueriedProject(bool isBillable)
            {
                const long projectId = 10;
                var project = new MockProject { Billable = isBillable };
                InteractorFactory.GetProjectById(Arg.Is(projectId))
                    .Execute()
                    .Returns(Observable.Return(project));

                var defaultsToBillable = await InteractorFactory.ProjectDefaultsToBillable(projectId).Execute();

                defaultsToBillable.Should().Be(isBillable);
            }
        }
    }
}
