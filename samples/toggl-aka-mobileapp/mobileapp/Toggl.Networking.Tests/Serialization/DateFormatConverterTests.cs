using FluentAssertions;
using Newtonsoft.Json;
using System;
using Toggl.Networking.Serialization.Converters;
using Toggl.Shared;
using Xunit;

namespace Toggl.Networking.Tests.Serialization
{
    public sealed class DateFormatConverterTests
    {
        public sealed class TheCanConvertMethod
        {
            private readonly DateFormatConverter converter = new DateFormatConverter();

            [Fact]
            public void ReturnsTrueWhenObjectTypeIsDateFormat()
            {
                converter.CanConvert(typeof(DateFormat)).Should().BeTrue();
            }

            [Theory]
            [InlineData(typeof(TimeFormat))]
            [InlineData(typeof(string))]
            [InlineData(typeof(DateTimeOffset))]
            public void ReturnsFalseForAnyOtherType(Type type)
            {
                converter.CanConvert(type).Should().BeFalse();
            }
        }

        public sealed class TheDateFormatStruct
        {
            private sealed class ClassWithDateFormat
            {
                [JsonConverter(typeof(DateFormatConverter))]
                public DateFormat DateFormat { get; set; }
            }

            private readonly string validJson = "{\"date_format\":\"MM.DD.YYYY\"}";
            private readonly ClassWithDateFormat validObject = new ClassWithDateFormat
            {
                DateFormat = DateFormat.FromLocalizedDateFormat("MM.DD.YYYY")
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
