using Toggl.Shared.Models;

namespace Toggl.Networking.Models
{
    internal sealed partial class Country : ICountry
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string CountryCode { get; set; }
    }
}
