using FluentAssertions;
using System;
using Toggl.Core.Tests.Generators;
using Toggl.Core.UI.ViewModels;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class OutdatedAppViewModelTests
    {
        public abstract class OutdatedAppViewModelTest : BaseViewModelTests<OutdatedAppViewModel>
        {
            protected override OutdatedAppViewModel CreateViewModel()
                => new OutdatedAppViewModel(PlatformInfo, RxActionFactory, NavigationService);
        }

        public sealed class TheConstructor : OutdatedAppViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool usePlatformInfo,
                bool useRxActionFactory,
                bool useNavigationService)
            {
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var platformInfo = usePlatformInfo ? PlatformInfo : null;
                var navigationService = useNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new OutdatedAppViewModel(
                        platformInfo, rxActionFactory, navigationService);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }
    }
}
