using FluentAssertions;
using NSubstitute;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Tests.Mocks;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.Interactors
{
    public class AreCustomColorsEnabledForWorkspaceInteractorTests
    {
        public sealed class TheAreCustomColorsEnabledForWorkspaceInteractor : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public async Task ChecksIfTheWorkspaceHasTheProFeature(bool hasFeature)
            {
                const long workspaceId = 11;
                var feature = new MockWorkspaceFeature { Enabled = hasFeature, FeatureId = WorkspaceFeatureId.Pro };
                var featureCollection = new MockWorkspaceFeatureCollection { Features = new[] { feature } };
                InteractorFactory.GetWorkspaceFeaturesById(workspaceId)
                    .Execute()
                    .Returns(Observable.Return(featureCollection));

                var hasProFeature = await InteractorFactory.AreCustomColorsEnabledForWorkspace(workspaceId).Execute();

                hasProFeature.Should().Be(hasFeature);
            }
        }
    }
}
