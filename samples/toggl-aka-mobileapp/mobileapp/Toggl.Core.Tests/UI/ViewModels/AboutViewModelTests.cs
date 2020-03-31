using FluentAssertions;
using NSubstitute;
using System;
using Toggl.Core.Tests.Generators;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class AboutViewModelTests
    {
        public abstract class AboutViewModelTest : BaseViewModelTests<AboutViewModel>
        {
            protected override AboutViewModel CreateViewModel()
                => new AboutViewModel(RxActionFactory, NavigationService);
        }

        public sealed class TheConstructor : AboutViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useNavigationService,
                bool useRxActionFactory)
            {
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var navigationService = useNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new AboutViewModel(
                        rxActionFactory,
                        navigationService);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheLicensesCommand : AboutViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void NavigatesToTheLicensesViewModel()
            {
                ViewModel.OpenLicensesView.Execute();

                NavigationService.Received().Navigate<LicensesViewModel>(ViewModel.View);
            }
        }
    }
}
