using FluentAssertions;
using Newtonsoft.Json;
using System;
using Toggl.Networking.Serialization.Converters;
using Toggl.Shared;
using Xunit;

namespace Toggl.Networking.Tests.Serialization
{
    public sealed class TimeFormatConverterTests
    {
        public sealed class TheCanConvertMethod
        {
            private readonly TimeFormatConverter converter = new TimeFormatConverter();

            [Fact]
            public void ReturnsTrueWhenObjectTypeIsDateFormat()
            {
                converter.CanConvert(typeof(TimeFormat)).Should().BeTrue();
            }

            [Theory]
            [InlineData(typeof(DateFormat))]
            [InlineData(typeof(string))]
            [InlineData(typeof(DateTimeOffset))]
            public void ReturnsFalseForAnyOtherType(Type type)
            {
                converter.CanConvert(type).Should().BeFalse();
            }
        }

        public sealed class TheTimeFormatStruct
        {
            private sealed class ClassWithTimeFormat
            {
                [JsonConverter(typeof(TimeFormatConverter))]
                public TimeFormat TimeOfDayFormat { get; set; }
            }

            private readonly string validJson = "{\"time_of_day_format\":\"h:mm A\"}";
            private readonly ClassWithTimeFormat validObject = new ClassWithTimeFormat
            {
                TimeOfDayFormat = TimeFormat.FromLocalizedTimeFormat("h:mm A")
            };

            [Fact]
            public void CanBeSerialized()
            {
                SerializationHelper.CanBeSerialized(validJson, validObject);
            }

            [Fact]
            public void CanBeDeserialized()
            {
                SerializationHelper.CanBeDeserialized(validJson, validObject);
            }
        }
    }
}
