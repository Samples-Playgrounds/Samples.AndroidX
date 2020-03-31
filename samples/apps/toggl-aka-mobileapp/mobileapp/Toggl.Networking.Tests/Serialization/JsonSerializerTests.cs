using FluentAssertions;
using System;
using Toggl.Networking.Serialization;
using Xunit;

namespace Toggl.Networking.Tests.Serialization
{
    public sealed class JsonSerializerTests
    {
        private class ThrowingModel
        {
            public string ThrowingProperty => throw new Exception();
        }

        private class TestModel
        {
            public string FooBar { get; set; }

            [IgnoreWhenPosting]
            public string IgnoredWhenPosting { get; set; }
        }

        public sealed class TheSerializeMethod
        {
            [Fact, LogIfTooSlow]
            public void CreatesSnakeCasedJson()
            {
                var testObject = new TestModel { FooBar = "Foo", IgnoredWhenPosting = "Baz" };
                const string expectedJson = "{\"foo_bar\":\"Foo\",\"ignored_when_posting\":\"Baz\"}";

                var jsonSerializer = new JsonSerializer();
                var actual = jsonSerializer.Serialize(testObject);

                actual.Should().Be(expectedJson);
            }

            [Fact, LogIfTooSlow]
            public void IgnoresPropertiesWithTheIgnoreWhenPostingAttribute()
            {
                var testObject = new TestModel { FooBar = "Foo", IgnoredWhenPosting = "Baz" };
                const string expectedJson = "{\"foo_bar\":\"Foo\"}";

                var jsonSerializer = new JsonSerializer();
                var actual = jsonSerializer.Serialize(testObject, SerializationReason.Post);

                actual.Should().Be(expectedJson);
            }

            [Fact, LogIfTooSlow]
            public void ThrowsSerializationExceptionIfSerializationThrows()
            {
                var serializar = new JsonSerializer();

                Action serialization = () => serializar.Serialize(new ThrowingModel());

                serialization.Should().Throw<SerializationException>();
            }
        }

        public sealed class TheDeserializeMethod
        {
            [Fact, LogIfTooSlow]
            public void ExpectsSnakeCasedJson()
            {
                const string testJson = "{\"foo_bar\":\"Foo\",\"ignored_when_posting\":\"Baz\"}";
                var expectedObject = new TestModel { FooBar = "Foo", IgnoredWhenPosting = "Baz" };

                var jsonSerializer = new JsonSerializer();
                var actual = jsonSerializer.Deserialize<TestModel>(testJson);

                actual.FooBar.Should().Be(expectedObject.FooBar);
            }

            [Theory, LogIfTooSlow]
            [InlineData("{")]
            [InlineData("}")]
            [InlineData("{\"FooBar\":}")]
            [InlineData("\"  \"")]
            [InlineData("This is an error.")]
            public void ThrowsDeserializationExceptionIfDeserializationThrows(string invalidJson)
            {
                var serializar = new JsonSerializer();

                Action deserialization = () => serializar.Deserialize<TestModel>(invalidJson);

                deserialization.Should().Throw<DeserializationException>();
            }
        }
    }
}
