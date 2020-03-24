using Toggl.Core.Models.Interfaces;
using Toggl.Shared;

namespace Toggl.Core.Tests.Mocks
{
    public sealed class MockWorkspaceFeature : IThreadSafeWorkspaceFeature
    {
        public WorkspaceFeatureId FeatureId { get; set; }

        public bool Enabled { get; set; }
    }
}
