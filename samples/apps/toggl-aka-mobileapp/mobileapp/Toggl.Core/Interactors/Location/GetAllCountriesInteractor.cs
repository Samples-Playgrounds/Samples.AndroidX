using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Shared;
using Toggl.Shared.Models;

namespace Toggl.Core.Interactors
{
    public class GetAllCountriesInteractor : IInteractor<IObservable<List<ICountry>>>
    {
        [Preserve(AllMembers = true)]
        private sealed class Country : ICountry
        {
            public long Id { get; set; }

            public string Name { get; set; }

            [JsonProperty("country_code")]
            public string CountryCode { get; set; }

            public Country() { }

            public Country(ICountry entity)
            {
                Id = entity.Id;
                Name = entity.Name;
                CountryCode = entity.CountryCode;
            }
        }

        public IObservable<List<ICountry>> Execute()
        {
            string json = Resources.CountriesJson;

            var countries = JsonConvert
                .DeserializeObject<List<Country>>(json)
                .OrderBy(country => country.Name)
                .ToList<ICountry>();

            return Observable.Return(countries);
        }
    }
}
