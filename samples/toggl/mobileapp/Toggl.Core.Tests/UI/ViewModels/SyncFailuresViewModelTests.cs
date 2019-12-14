using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Core.Interactors;
using Toggl.Core.Models;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.UI.ViewModels;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class SyncFailuresViewModelTests
    {
        public abstract class SyncFailuresViewModelTest : BaseViewModelTests<SyncFailuresViewModel>
        {
            protected override SyncFailuresViewModel CreateViewModel()
            {
                var vm = new SyncFailuresViewModel(InteractorFactory, NavigationService);
                return vm;
            }
        }

        public sealed class TheConstructor : SyncFailuresViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useInteractorFactory,
                bool useNavigationService)
            {
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var navigationService = useNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new SyncFailuresViewModel(interactorFactory, navigationService);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheSyncFailuresProperty : SyncFailuresViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ExecutesGetItemsThatFailedInteractor()
            {
                var mockedInteractor = Substitute.For<IInteractor<IObservable<IEnumerable<SyncFailureItem>>>>();
                InteractorFactory.GetItemsThatFailedToSync().Returns(mockedInteractor);

                ViewModel.Initialize();

                mockedInteractor.Received().Execute();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsTheInteractorResultAsAList()
            {
                var mockedInteractor = Substitute.For<IInteractor<IObservable<IEnumerable<SyncFailureItem>>>>();
                InteractorFactory.GetItemsThatFailedToSync().Returns(mockedInteractor);

                SyncFailureItem[] result =
                {
                    new SyncFailureItem(new MockProject { Name = "MyProject" }),
                    new SyncFailureItem(new MockProject { Name = "MyProject 2" }),
                    new SyncFailureItem(new MockTag { Name = "MyTag" })
                };

                mockedInteractor.Execute().Returns(Observable.Return(result));

                ViewModel.Initialize();

                ViewModel.SyncFailures.Should().BeEquivalentTo(result);
            }
        }
    }
}
