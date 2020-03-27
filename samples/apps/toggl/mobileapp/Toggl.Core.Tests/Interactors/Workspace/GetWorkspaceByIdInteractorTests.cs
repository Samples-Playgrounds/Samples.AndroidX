using FluentAssertions;
using NSubstitute;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Tests.Mocks;
using Xunit;

namespace Toggl.Core.Tests.Interactors
{
    public class GetWorkspaceByIdInteractorTests
    {
        public sealed class TheGetWorkspaceByIdInteractor : BaseInteractorTests
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsTheWorkspaceWithPassedId()
            {
                const long workspaceId = 10;
                var workspace = new MockWorkspace();
                InteractorFactory.GetWorkspaceById(workspaceId)
                    .Execute()
                    .Returns(Observable.Return(workspace));

                var returnedWorkspace = await InteractorFactory.GetWorkspaceById(workspaceId).Execute();

                returnedWorkspace.Should().BeSameAs(workspace);
            }
        }
    }
}
