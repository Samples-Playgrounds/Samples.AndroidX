using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.ViewModels;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class NoWorkspaceViewModelTests
    {
        public abstract class NoWorkspaceViewModelTest : BaseViewModelTests<NoWorkspaceViewModel>
        {
            protected override NoWorkspaceViewModel CreateViewModel()
                => new NoWorkspaceViewModel(SyncManager, InteractorFactory, NavigationService, AccessRestrictionStorage, SchedulerProvider, RxActionFactory);
        }

        public sealed class TheConstructor : NoWorkspaceViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useSyncManager,
                bool useAccessRestrictionStorage,
                bool useInteractorFactory,
                bool useNavigationService,
                bool useSchedulerProvider,
                bool useRxActionFactory)
            {
                var syncManager = useSyncManager ? SyncManager : null;
                var accessRestrictionStorage = useAccessRestrictionStorage ? AccessRestrictionStorage : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new NoWorkspaceViewModel(syncManager, interactorFactory, navigationService, accessRestrictionStorage, schedulerProvider, rxActionFactory);

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheTryAgainCommand : NoWorkspaceViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesWhenAnotherWorkspaceIsFetched()
            {
                var workspace = Substitute.For<IThreadSafeWorkspace>();
                InteractorFactory.GetAllWorkspaces().Execute().Returns(Observable.Return(new List<IThreadSafeWorkspace>() { workspace }));

                ViewModel.TryAgain.Execute();
                TestScheduler.Start();

                View.Received().Close();
            }

            [Fact, LogIfTooSlow]

            public async Task ResetsNoWorkspaceStateWhenAnotherWorkspaceIsFetched()
            {
                var workspace = Substitute.For<IThreadSafeWorkspace>();
                InteractorFactory.GetAllWorkspaces().Execute().Returns(Observable.Return(new List<IThreadSafeWorkspace>() { workspace }));

                ViewModel.TryAgain.Execute();
                TestScheduler.Start();

                AccessRestrictionStorage.Received().SetNoWorkspaceStateReached(Arg.Is(false));
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNothingWhenNoWorkspacesAreFetched()
            {
                DataSource.Workspaces.GetAll().Returns(Observable.Return(new List<IThreadSafeWorkspace>()));

                ViewModel.TryAgain.Execute();
                TestScheduler.Start();

                View.DidNotReceive().Close();
                AccessRestrictionStorage.DidNotReceive().SetNoWorkspaceStateReached(Arg.Any<bool>());
            }

            [Fact, LogIfTooSlow]
            public async Task StartsAndStopsLoading()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsLoading.Subscribe(observer);

                var workspace = Substitute.For<IThreadSafeWorkspace>();
                DataSource.Workspaces.GetAll().Returns(Observable.Return(new List<IThreadSafeWorkspace>() { workspace }));

                ViewModel.CreateWorkspaceWithDefaultName.Execute();
                TestScheduler.Start();

                observer.Values().AssertEqual(
                    false,
                    true,
                    false
                );
            }
        }

        public sealed class TheCreateWorkspaceCommand : NoWorkspaceViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task CreatesNewWorkspaceWithDefaultName()
            {
                var name = "Rick Sanchez";
                var user = Substitute.For<IThreadSafeUser>();
                user.Fullname.Returns(name);
                DataSource.User.Current.Returns(Observable.Return(user));

                ViewModel.CreateWorkspaceWithDefaultName.Execute();
                TestScheduler.Start();

                await InteractorFactory.CreateDefaultWorkspace().Received().Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task ClosesAfterNewWorkspaceIsCreated()
            {
                var workspace = Substitute.For<IThreadSafeWorkspace>();
                InteractorFactory.CreateDefaultWorkspace().Execute().Returns(Observable.Return(Unit.Default));
                DataSource.Workspaces.GetAll().Returns(Observable.Return(new List<IThreadSafeWorkspace> { workspace }));

                ViewModel.CreateWorkspaceWithDefaultName.Execute();
                TestScheduler.Start();

                View.Received().Close();
            }

            [Fact, LogIfTooSlow]
            public async Task ResetsNoWorkspaceStateWhenAfterNewWorkspaceIsCreated()
            {
                var workspace = Substitute.For<IThreadSafeWorkspace>();
                InteractorFactory.CreateDefaultWorkspace().Execute().Returns(Observable.Return(Unit.Default));
                DataSource.Workspaces.GetAll().Returns(Observable.Return(new List<IThreadSafeWorkspace> { workspace }));

                ViewModel.CreateWorkspaceWithDefaultName.Execute();
                TestScheduler.Start();

                AccessRestrictionStorage.Received().SetNoWorkspaceStateReached(Arg.Is(false));
            }

            [Fact, LogIfTooSlow]
            public async Task StartsAndStopsLoading()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsLoading.Subscribe(observer);
                InteractorFactory.CreateDefaultWorkspace().Execute().Returns(Observable.Return(Unit.Default));

                ViewModel.CreateWorkspaceWithDefaultName.Execute();
                TestScheduler.Start();

                observer.Values().AssertEqual(
                    false,
                    true,
                    false
                );
            }
        }
    }
}
