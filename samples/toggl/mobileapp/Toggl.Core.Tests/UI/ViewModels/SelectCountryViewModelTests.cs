using FluentAssertions;
using FsCheck;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Tests.Generators;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared.Models;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class SelectCountryViewModelTests
    {
        public abstract class SelectCountryViewModelTest : BaseViewModelTests<SelectCountryViewModel, long?, long?>
        {
            protected override SelectCountryViewModel CreateViewModel()
                => new SelectCountryViewModel(NavigationService, RxActionFactory);

            protected List<ICountry> GenerateCountriesList() =>
                Enumerable.Range(1, 10).Select(i =>
                {
                    var country = Substitute.For<ICountry>();
                    country.Id.Returns(i);
                    country.Name.Returns(i.ToString());
                    country.CountryCode.Returns(i.ToString());
                    return country;
                }).ToList();
        }

        public sealed class TheConstructor : SelectCountryViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useNavigationService,
                bool useRxActionFactory)
            {
                var navigationService = useNavigationService ? NavigationService : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new SelectCountryViewModel(navigationService, rxActionFactory);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheInitializeMethod : SelectCountryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task AddsAllCountriesToTheListOfSuggestions()
            {
                await ViewModel.Initialize(10);

                var countries = await ViewModel.Countries.FirstAsync();
                countries.Count().Should().Equals(250);
            }

            [Theory, LogIfTooSlow]
            [InlineData(1)]
            [InlineData(150)]
            [InlineData(200)]
            public async Task SetsTheAppropriateCountryAsTheCurrentlySelectedOne(int id)
            {
                await ViewModel.Initialize(id);

                var countries = await ViewModel.Countries.FirstAsync();
                countries.Single(c => c.Selected).Country.Id.Should().Be(id);
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotSetTheSelectedCountryIfPreparingWithNull()
            {
                await ViewModel.Initialize(null);

                var countries = await ViewModel.Countries.FirstAsync();
                countries.All(suggestion => !suggestion.Selected);
            }
        }

        public sealed class TheSelectCountryCommand : SelectCountryViewModelTest
        {
            public TheSelectCountryCommand()
            {
                ViewModel.Initialize(10);
            }

            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModelWithSelectedCountryCode()
            {
                var country = Substitute.For<ICountry>();
                country.Id.Returns(1);
                country.Name.Returns("Greece");
                country.CountryCode.Returns("GR");

                await ViewModel.Initialize(null);

                var selectableCountry = new SelectableCountryViewModel(country, true);

                ViewModel.SelectCountry.Execute(selectableCountry);

                TestScheduler.Start();
                (await ViewModel.Result).Should().Be(country.Id);
            }
        }

        public sealed class TheTextProperty : SelectCountryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task FiltersTheSuggestionsWhenItChanges()
            {
                await ViewModel.Initialize(10);

                ViewModel.FilterText.OnNext("Greece");

                var countries = await ViewModel.Countries.FirstAsync();
                countries.Count().Should().Equals(1);
            }
        }
    }
}
