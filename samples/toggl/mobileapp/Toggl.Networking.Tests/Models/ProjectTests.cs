using FluentAssertions;
using System;
using Toggl.Networking.Models;
using Toggl.Networking.Serialization;
using Xunit;

namespace Toggl.Networking.Tests.Models
{
    public sealed class ProjectTests
    {
        private string validJson
            => "{\"id\":64261173,\"workspace_id\":376665,\"client_id\":null,\"name\":\"Test project\",\"is_private\":false,\"active\":true,\"at\":\"2016-05-20T17:28:00+00:00\",\"server_deleted_at\":null,\"color\":\"#f61d38\",\"billable\":false,\"template\":false,\"auto_estimates\":false,\"estimated_hours\":null,\"rate\":null,\"currency\":null,\"actual_hours\":277}";

        private Project validProject => new Project
        {
            Id = 64261173,
            WorkspaceId = 376665,
            Name = "Test project",
            IsPrivate = false,
            Active = true,
            At = new DateTimeOffset(2016, 5, 20, 17, 28, 0, TimeSpan.Zero),
            Color = "#F61D38",
            Billable = false,
            Template = false,
            AutoEstimates = false,
            ActualHours = 277
        };

        [Fact, LogIfTooSlow]
        public void HasConstructorWhichCopiesValuesFromInterfaceToTheNewInstance()
        {
            var clonedObject = new Project(validProject);

            clonedObject.Should().NotBeSameAs(validProject);
            clonedObject.Should().BeEquivalentTo(validProject, options => options.IncludingProperties());
        }

        [Fact, LogIfTooSlow]
        public void CanBeDeserialized()
        {
            SerializationHelper.CanBeDeserialized(validJson, validProject);
        }

        [Fact, LogIfTooSlow]
        public void CanBeSerialized()
        {
            SerializationHelper.CanBeSerialized(validJson, validProject);
        }

        [Fact, LogIfTooSlow]
        public void ColorIsAlwaysLowercaseWhenSerialized()
        {
            var serializer = new JsonSerializer();
            var json = serializer.Serialize(validProject);

            json.Should().MatchRegex("^.*\"color\":\"#f61d38\".*$");
            json.Should().NotMatchRegex("^.*\"color\":\"#F61D38\".*$");
        }
    }
}
