using System;
using Toggl.Core.UI.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Models;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SelectableCountryViewModel : IDiffableByIdentifier<SelectableCountryViewModel>
    {
        public ICountry Country { get; }

        public bool Selected { get; }

        public SelectableCountryViewModel(ICountry country, bool selected)
        {
            Country = country;
            Selected = selected;
        }

        public override string ToString() => Country.Name;

        public long Identifier => Country.Id;

        public bool Equals(SelectableCountryViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Country, other.Country) && Selected == other.Selected;
        }

        public override bool Equals(object obj) => Equals(obj as SelectableCountryViewModel);

        public override int GetHashCode() => HashCode.Combine(Country, Selected);
    }
}
