using FluentAssertions;
using System;
using Toggl.Networking.Models;
using Xunit;

namespace Toggl.Networking.Tests.Models
{
    public sealed class TaskTests
    {
        private string validJson
            => "{\"id\":79,\"name\":\"new task\",\"project_id\":1,\"workspace_id\":56,\"user_id\":null,\"estimated_seconds\":13,\"active\":true,\"at\":\"2017-01-01T02:03:00+00:00\",\"tracked_seconds\":12}";

        private Task validTask => new Task
        {
            Id = 79,
            Name = "new task",
            ProjectId = 1,
            WorkspaceId = 56,
            UserId = null,
            EstimatedSeconds = 13,
            Active = true,
            At = new DateTimeOffset(2017, 1, 1, 2, 3, 0, TimeSpan.Zero),
            TrackedSeconds = 12
        };

        [Fact, LogIfTooSlow]
        public void HasConstructorWhichCopiesValuesFromInterfaceToTheNewInstance()
        {
            var clonedObject = new Task(validTask);

            clonedObject.Should().NotBeSameAs(validTask);
            clonedObject.Should().BeEquivalentTo(validTask, options => options.IncludingProperties());
        }

        [Fact, LogIfTooSlow]
        public void CanBeDeserialized()
        {
            SerializationHelper.CanBeDeserialized(validJson, validTask);
        }

        [Fact, LogIfTooSlow]
        public void CanBeSerialized()
        {
            SerializationHelper.CanBeSerialized(validJson, validTask);
        }
    }
}
