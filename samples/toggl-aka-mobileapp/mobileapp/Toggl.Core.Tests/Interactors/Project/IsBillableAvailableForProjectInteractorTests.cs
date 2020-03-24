using FluentAssertions;
using NSubstitute;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Tests.Mocks;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.Interactors
{
    public class IsBillableAvailableForProjectInteractorTests
    {
        public sealed class TheIsBillableAvailableForProjectInteractor : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public async Task ChecksIfTheProjectsWorkspaceHasTheProFeature(bool hasFeature)
            {
                const long projectId = 10;
                const long workspaceId = 11;
                var project = new MockProject { WorkspaceId = workspaceId };
                var feature = new MockWorkspaceFeature { Enabled = hasFeature, FeatureId = WorkspaceFeatureId.Pro };
                var featureCollection = new MockWorkspaceFeatureCollection { Features = new[] { feature } };
                InteractorFactory.GetWorkspaceFeaturesById(workspaceId)
                    .Execute()
                    .Returns(Observable.Return(featureCollection));
                InteractorFactory.GetProjectById(Arg.Is(projectId))
                    .Execute()
                    .Returns(Observable.Return(project));

                var hasProFeature = await InteractorFactory.IsBillableAvailableForProject(projectId).Execute();

                hasProFeature.Should().Be(hasFeature);
            }
        }
    }
}
