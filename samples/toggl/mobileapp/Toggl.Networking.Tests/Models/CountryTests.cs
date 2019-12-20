using FluentAssertions;
using Toggl.Networking.Models;
using Xunit;

namespace Toggl.Networking.Tests.Models
{
    public sealed class CountryTests
    {
        private string validJson
            => "{\"id\":235,\"name\":\"United States\",\"vat_applicable\":false,\"vat_regex\":null,\"vat_percentage\":null,\"country_code\":\"US\"}";

        private Country validCountry => new Country
        {
            Id = 235,
            Name = "United States",
            CountryCode = "US"
        };

        [Fact, LogIfTooSlow]
        public void HasConstructorWhichCopiesValuesFromInterfaceToTheNewInstance()
        {
            var clonedObject = new Country(validCountry);

            clonedObject.Should().NotBeSameAs(validCountry);
            clonedObject.Should().BeEquivalentTo(validCountry, options => options.IncludingProperties());
        }

        [Fact, LogIfTooSlow]
        public void CanBeDeserialized()
        {
            SerializationHelper.CanBeDeserialized(validJson, validCountry);
        }
    }
}
