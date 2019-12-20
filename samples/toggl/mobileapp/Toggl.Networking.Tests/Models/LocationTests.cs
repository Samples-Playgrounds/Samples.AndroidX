using FluentAssertions;
using System;
using Toggl.Networking.Models;
using Toggl.Networking.Serialization;
using Xunit;

namespace Toggl.Networking.Tests.Models
{
    public sealed class LocationTests
    {
        public sealed class TheLocationModel
        {
            private string validJson
                = "{\"city\":\"Riga\",\"state\":\"Riga\",\"country_name\":\"Latvia\",\"country_code\":\"LV\"}";

            private Location validLocation = new Location
            {
                CountryName = "Latvia",
                CountryCode = "LV"
            };

            [Fact, LogIfTooSlow]
            public void HasConstructorWhichCopiesValuesFromInterfaceToTheNewInstance()
            {
                var clonedObject = new Location(validLocation);

                clonedObject.Should().NotBeSameAs(validLocation);
                clonedObject.Should().BeEquivalentTo(validLocation, options => options.IncludingProperties());
            }

            [Fact, LogIfTooSlow]
            public void CanBeDeserialized()
            {
                SerializationHelper.CanBeDeserialized(validJson, validLocation);
            }

            [Fact, LogIfTooSlow]
            public void CannotBeSerialized()
            {
                Action serializing = () => SerializationHelper.CanBeSerialized(validJson, validLocation);

                serializing.Should().Throw<SerializationException>();
            }
        }
    }
}
