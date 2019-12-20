using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Interactors;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SelectCountryViewModel : ViewModel<long?, long?>
    {
        public IObservable<IImmutableList<SelectableCountryViewModel>> Countries { get; private set; }
        public ISubject<string> FilterText { get; } = new BehaviorSubject<string>(string.Empty);
        public InputAction<SelectableCountryViewModel> SelectCountry { get; }

        public SelectCountryViewModel(INavigationService navigationService, IRxActionFactory rxActionFactory)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            SelectCountry = rxActionFactory.FromAction<SelectableCountryViewModel>(selectCountry);
        }

        public override async Task Initialize(long? selectedCountryId)
        {
            await base.Initialize(selectedCountryId);

            var allCountries = await new GetAllCountriesInteractor().Execute();

            var selectedElement = allCountries.Find(c => c.Id == selectedCountryId);
            if (selectedElement != null)
            {
                allCountries.Remove(selectedElement);
                allCountries.Insert(0, selectedElement);
            }

            Countries = FilterText
                .Select(text => text?.Trim() ?? string.Empty)
                .DistinctUntilChanged()
                .Select(trimmedText =>
                    allCountries
                        .Where(c => c.Name.ContainsIgnoringCase(trimmedText))
                        .Select(c => new SelectableCountryViewModel(c, c.Id == selectedCountryId))
                        .ToImmutableList());
        }

        private void selectCountry(SelectableCountryViewModel selectedCountry)
        {
            Close(selectedCountry.Country.Id);
        }
    }
}
