﻿using FluentAssertions;
using NSubstitute;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Exceptions;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class SelectDefaultWorkspaceViewModelTests
    {
        public abstract class SelectDefaultWorkspaceViewModelTest : BaseViewModelTests<SelectDefaultWorkspaceViewModel>
        {
            protected override SelectDefaultWorkspaceViewModel CreateViewModel()
                => new SelectDefaultWorkspaceViewModel(DataSource, InteractorFactory, NavigationService, AccessRestrictionStorage, RxActionFactory);
        }

        public sealed class TheConstructor : SelectDefaultWorkspaceViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useDataSource,
                bool useInteractorFactory,
                bool useNavigationService,
                bool useAccessRestrictionStorage,
                bool useRxActionFactory)
            {
                Action tryingToConstructWithEmptyParameters = ()
                    => new SelectDefaultWorkspaceViewModel(
                        useDataSource ? DataSource : null,
                        useInteractorFactory ? InteractorFactory : null,
                        useNavigationService ? NavigationService : null,
                        useAccessRestrictionStorage ? AccessRestrictionStorage : null,
                        useRxActionFactory ? RxActionFactory : null);

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheInitializeMethod : SelectDefaultWorkspaceViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task FillsTheWorkspaceList()
            {
                var workspaceCount = 10;
                var workspaceIds = Enumerable
                    .Range(0, workspaceCount)
                    .Select(id => (long)id);
                var workspaces = workspaceIds
                    .Select(id => new MockWorkspace { Id = id, Name = id.ToString() })
                    .Apply(Observable.Return);
                DataSource.Workspaces.GetAll().Returns(workspaces);

                await ViewModel.Initialize();

                ViewModel
                    .Workspaces
                    .Should()
                    .OnlyContain(workspace => workspaceIds.Contains(workspace.WorkspaceId));
            }

            [Fact, LogIfTooSlow]
            public void ThrowsNoWorkspaceExceptionIfThereAreNoWorkspaces()
            {
                DataSource.Workspaces.GetAll().Returns(Observable.Return(new IThreadSafeWorkspace[0]));

                Action initialization = () => ViewModel.Initialize().Wait();

                initialization.Should().Throw<NoWorkspaceException>();
            }
        }

        public sealed class SelectWorkspaceAction : SelectDefaultWorkspaceViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task SetsTheWorkspaceAsDefault()
            {
                var workspaceCount = 10;
                var workspaceIds = Enumerable
                    .Range(0, workspaceCount)
                    .Select(id => (long)id);
                var workspaces = workspaceIds
                    .Select(id => new MockWorkspace { Id = id, Name = id.ToString() })
                    .Apply(Observable.Return);
                DataSource.Workspaces.GetAll().Returns(workspaces);
                var setDefaultWorkspaceInteractor = Substitute.For<IInteractor<IObservable<Unit>>>();
                InteractorFactory.SetDefaultWorkspace(Arg.Any<long>()).Returns(setDefaultWorkspaceInteractor);
                await ViewModel.Initialize();
                var selectedWorkspace = ViewModel.Workspaces.First();

                ViewModel.SelectWorkspace.Execute(selectedWorkspace);
                TestScheduler.Start();

                InteractorFactory.Received().SetDefaultWorkspace(selectedWorkspace.WorkspaceId);
                await setDefaultWorkspaceInteractor.Received().Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModel()
            {
                var selectedWorkspace = new SelectableWorkspaceViewModel(new MockWorkspace(), false);

                ViewModel.SelectWorkspace.Execute(selectedWorkspace);
                TestScheduler.Start();

                View.Received().Close();
            }
        }
    }
}
