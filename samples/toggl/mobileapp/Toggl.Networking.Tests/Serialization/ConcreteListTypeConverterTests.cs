using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Toggl.Networking.Serialization;
using Toggl.Networking.Serialization.Converters;
using Xunit;
using JsonSerializer = Toggl.Networking.Serialization.JsonSerializer;

namespace Toggl.Networking.Tests.Serialization
{
    public sealed class ConcreteListTypeConverterTests
    {
        [Fact, LogIfTooSlow]
        public void SerializationMustUseTheConverter()
        {
            var output = serializer.Serialize(SomeItemsContainer);

            output.Should().Be(JsonArray);
        }

        [Fact, LogIfTooSlow]
        public void DeserializationMustUseTheConverter()
        {
            var output = serializer.Deserialize<OtherClass>(JsonArray);

            output.Should().BeEquivalentTo(SomeItemsContainer, options => options.IncludingProperties());
        }

        [Fact, LogIfTooSlow]
        public void DeserializationFailsWithoutTheConverter()
        {
            Action deserialization = () => serializer.Deserialize<DifferentClass>(JsonArray);

            deserialization.Should().Throw<DeserializationException>();
        }

        private JsonSerializer serializer => new JsonSerializer();

        private interface ISomeInterface
        {
            string SomeName { get; }
        }

        private class SomeClass : ISomeInterface
        {
            public string SomeName { get; set; }
        }

        private class OtherClass
        {
            [JsonConverter(typeof(ConcreteListTypeConverter<SomeClass, ISomeInterface>))]
            public List<ISomeInterface> SomeItems { get; set; }
        }

        private class DifferentClass
        {
            public List<ISomeInterface> SomeItems { get; set; }
        }

        private const string JsonArray = "{\"some_items\":[{\"some_name\":\"A\"},{\"some_name\":\"B\"}]}";

        private static readonly OtherClass SomeItemsContainer = new OtherClass
        {
            SomeItems = new List<ISomeInterface>
            {
                new SomeClass { SomeName = "A" },
                new SomeClass { SomeName = "B" }
            }
        };
    }
}
