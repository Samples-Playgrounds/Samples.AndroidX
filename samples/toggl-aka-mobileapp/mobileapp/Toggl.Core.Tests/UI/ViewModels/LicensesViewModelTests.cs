using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Toggl.Core.Tests.Generators;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class LicensesViewModelTests
    {
        public abstract class LicensesViewModelTest : BaseViewModelTests<LicensesViewModel>
        {
            protected override LicensesViewModel CreateViewModel()
                => new LicensesViewModel(LicenseProvider, NavigationService);
        }

        public sealed class TheConstructor : LicensesViewModelTest
        {
            private readonly Dictionary<string, string> licenses = new Dictionary<string, string>
            {
                { "Something", "Some long license" },
                { "Something else", "Some other license" },
                { "Third one", "Another even longer license" }
            };

            protected override LicensesViewModel CreateViewModel()
            {
                LicenseProvider.GetAppLicenses().Returns(licenses);

                return base.CreateViewModel();
            }

            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useLicenseProvider,
                bool useNavigationService)
            {

                var licenseProvider = useLicenseProvider ? LicenseProvider : null;
                var navigationService = useNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameters =
                     () => new LicensesViewModel(licenseProvider, navigationService);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }

            [Fact, LogIfTooSlow]
            public void InitializesTheLicenses()
            {
                var expectedLicenses = licenses
                    .Select(license => new License(license.Key, license.Value))
                    .ToImmutableList();

                ViewModel.Licenses.Should().BeEquivalentTo(expectedLicenses);
            }
        }
    }
}
