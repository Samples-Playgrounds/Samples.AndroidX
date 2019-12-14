using FluentAssertions;
using System;
using Toggl.Networking.Models;
using Xunit;

namespace Toggl.Networking.Tests.Models
{
    public sealed class ClientTests
    {
        public sealed class TheClientModel
        {
            private string validJson
                => "{\"id\":23741667,\"wid\":1427273,\"name\":\"Test\",\"at\":\"2014-04-25T10:10:13+00:00\",\"server_deleted_at\":null}";

            private Client validClient => new Client
            {
                Id = 23741667,
                WorkspaceId = 1427273,
                Name = "Test",
                At = new DateTimeOffset(2014, 04, 25, 10, 10, 13, TimeSpan.Zero),
                ServerDeletedAt = null
            };

            [Fact, LogIfTooSlow]
            public void HasConstructorWhichCopiesValuesFromInterfaceToTheNewInstance()
            {
                var clonedObject = new Client(validClient);

                clonedObject.Should().NotBeSameAs(validClient);
                clonedObject.Should().BeEquivalentTo(validClient, options => options.IncludingProperties());
            }

            [Fact, LogIfTooSlow]
            public void CanBeDeserialized()
            {
                SerializationHelper.CanBeDeserialized(validJson, validClient);
            }

            [Fact, LogIfTooSlow]
            public void CanBeSerialized()
            {
                SerializationHelper.CanBeSerialized(validJson, validClient);
            }
        }
    }
}
