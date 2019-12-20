using FluentAssertions;
using Toggl.Networking.Serialization;

namespace Toggl.Networking.Tests
{
    public static class SerializationHelper
    {
        private static readonly JsonSerializer serializer = new JsonSerializer();

        internal static void CanBeDeserialized<T>(string validJson, T validObject)
        {
            var actual = serializer.Deserialize<T>(validJson);

            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(validObject);
        }

        internal static void CanBeSerialized<T>(string validJson, T validObject, SerializationReason reason = SerializationReason.Default)
        {
            var actualJson = serializer.Serialize(validObject, reason);

            actualJson.Should().Be(validJson);
        }
    }
}
