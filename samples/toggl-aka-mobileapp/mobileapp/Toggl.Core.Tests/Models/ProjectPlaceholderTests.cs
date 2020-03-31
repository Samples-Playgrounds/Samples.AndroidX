using FluentAssertions;
using Xunit;

namespace Toggl.Core.Tests
{
    public sealed class ProjectPlaceholder
    {
        [Fact]
        public void TheCreatedProjectPlaceholderIsNotActive()
        {
            var project = Models.Project.CreatePlaceholder(123, 456);

            project.Id.Should().Be(123);
            project.WorkspaceId.Should().Be(456);
            project.Active.Should().BeFalse();
        }
    }
}
