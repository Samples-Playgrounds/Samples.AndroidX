using FluentAssertions;
using System;
using Toggl.Core.Tests.Generators;
using Toggl.Core.UI.ViewModels;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class TermsOfServiceViewModelTests
    {
        public abstract class TermsOfServiceViewModelTest : BaseViewModelWithOutputTests<TermsOfServiceViewModel, bool>
        {
            protected override TermsOfServiceViewModel CreateViewModel()
                => new TermsOfServiceViewModel(RxActionFactory, NavigationService);
        }

        public sealed class TheConstructor : TermsOfServiceViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useRxActionFactory,
                bool useNavigationService)
            {
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var navigationService = useNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new TermsOfServiceViewModel(rxActionFactory, navigationService);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }
    }
}
