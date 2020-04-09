using Toggl.Shared.Models;

namespace Toggl.Core.Models
{
    public sealed class Country : ICountry
    {
        public string Name { get; }

        public string CountryCode { get; }

        public long Id { get; }

        public Country(string name, string countryCode, long id)
        {
            Id = id;
            Name = name;
            CountryCode = countryCode;
        }
    }
}
