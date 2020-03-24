using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.DTOs;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xunit;
using ProjectPredicate = System.Func<Toggl.Storage.Models.IDatabaseProject, bool>;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class EditProjectViewModelTests
    {
        public abstract class EditProjectViewModelTest : BaseViewModelTests<EditProjectViewModel, string, long?>
        {
            protected const long DefaultWorkspaceId = 10;
            protected const string DefaultWorkspaceName = "Some workspace name";
            private const long otherWorkspaceId = DefaultWorkspaceId + 1;
            private const long projectId = 12345;
            protected string ProjectName { get; } = "A random project";
            protected IThreadSafeWorkspace Workspace { get; } = Substitute.For<IThreadSafeWorkspace>();

            protected void SetupDataSourceToReturnExistingProjectsAndDefaultWorkspace(bool dataSourceProjectIsInSameWorkspace)
            {
                var project = Substitute.For<IThreadSafeProject>();
                project.Id.Returns(projectId);
                project.Name.Returns(ProjectName);
                project.WorkspaceId.Returns(dataSourceProjectIsInSameWorkspace ? DefaultWorkspaceId : otherWorkspaceId);

                var defaultWorkspace = Substitute.For<IThreadSafeWorkspace>();
                defaultWorkspace.Id.Returns(DefaultWorkspaceId);
                defaultWorkspace.Name.Returns(Guid.NewGuid().ToString());

                InteractorFactory
                    .GetDefaultWorkspace()
                    .Execute()
                    .Returns(Observable.Return(defaultWorkspace));

                InteractorFactory
                    .AreCustomColorsEnabledForWorkspace(DefaultWorkspaceId)
                    .Execute()
                    .Returns(Observable.Return(false));

                DataSource.Projects
                    .GetAll(Arg.Any<ProjectPredicate>())
                    .Returns(callInfo => Observable.Return(new[] { project })
                        .Select(projects => projects.Where<IThreadSafeProject>(callInfo.Arg<ProjectPredicate>())));
            }
            protected EditProjectViewModelTest()
            {
                ViewModel.Initialize("A valid name");
            }

            protected override EditProjectViewModel CreateViewModel()
                => new EditProjectViewModel(
                    DataSource,
                    RxActionFactory,
                    InteractorFactory,
                    SchedulerProvider,
                    NavigationService
                );
        }

        public abstract class WorkspaceChangeAwareTests : EditProjectViewModelTest
        {
            protected void SetupDataSourceToReturnMultipleWorkspaces()
            {
                List<IThreadSafeWorkspace> workspaces = new List<IThreadSafeWorkspace>();
                List<IThreadSafeProject> projects = new List<IThreadSafeProject>();

                for (long workspaceId = 0; workspaceId < 2; workspaceId++)
                {
                    var workspace = Substitute.For<IThreadSafeWorkspace>();
                    workspace.Id.Returns(workspaceId);
                    workspace.Name.Returns(Guid.NewGuid().ToString());
                    workspaces.Add(workspace);

                    InteractorFactory
                        .GetWorkspaceById(workspaceId)
                        .Execute()
                        .Returns(Observable.Return(workspace));

                    for (long projectId = 0; projectId < 3; projectId++)
                    {
                        var project = Substitute.For<IThreadSafeProject>();
                        project.Id.Returns(10 * workspaceId + projectId);
                        project.Name.Returns($"Project-{workspaceId}-{projectId}");
                        project.WorkspaceId.Returns(workspaceId);
                        projects.Add(project);
                    }

                    var sameNameProject = Substitute.For<IThreadSafeProject>();
                    sameNameProject.Id.Returns(10 + workspaceId);
                    sameNameProject.Name.Returns("Project");
                    sameNameProject.WorkspaceId.Returns(workspaceId);
                    projects.Add(sameNameProject);
                }

                var defaultWorkspace = workspaces[0];

                InteractorFactory
                    .GetDefaultWorkspace()
                    .Execute()
                    .Returns(Observable.Return(defaultWorkspace));

                InteractorFactory
                    .AreCustomColorsEnabledForWorkspace(Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(false));

                DataSource.Projects
                    .GetAll(Arg.Any<ProjectPredicate>())
                    .Returns(callInfo =>
                        Observable.Return(projects)
                            .Select(p => p.Where<IThreadSafeProject>(callInfo.Arg<ProjectPredicate>())));

                View.Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<IThreadSafeWorkspace>>>(),
                    Arg.Any<int>())
                    .Returns(Observable.Return(new MockWorkspace { Id = 1L }));
            }
        }

        public sealed class TheConstructor : EditProjectViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useDataSource,
                bool useRxActionFactory,
                bool useInteractorFactory,
                bool useSchedulerProvider,
                bool useNavigationService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
            
                Action tryingToConstructWithEmptyParameters =
                    () => new EditProjectViewModel(
                        dataSource,
                        rxActionFactory,
                        interactorFactory,
                        schedulerProvider,
                        navigationService
                    );

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheSaveEnabledProperty : WorkspaceChangeAwareTests
        {
            private ITestableObserver<bool> saveEnabledObserver;

            protected override void AdditionalViewModelSetup()
            {
                saveEnabledObserver = TestScheduler.CreateObserver<bool>();
                ViewModel.Name.Subscribe();
                ViewModel.Save.Enabled.Subscribe(saveEnabledObserver);
            }

            [Fact, LogIfTooSlow]
            public void IsFalseWhenTheNameIsEmpty()
            {
                TestScheduler.Start();
                ViewModel.Name.Accept("");
                TestScheduler.Start();

                saveEnabledObserver.LastEmittedValue().Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void IsFalseWhenTheNameIsJustWhiteSpace()
            {
                TestScheduler.Start();
                ViewModel.Name.Accept("            ");
                TestScheduler.Start();

                saveEnabledObserver.LastEmittedValue().Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void IsFalseWhenTheNameIsLongerThanTheThresholdInBytes()
            {
                TestScheduler.Start();
                ViewModel.Name.Accept("This is a ridiculously big project name made solely with the purpose of testing whether or not Toggl apps UI has validation logic that prevents such a large name to be persisted or, even worse, pushed to the api, an event that might end up in crashes and whatnot");
                TestScheduler.Start();

                saveEnabledObserver.LastEmittedValue().Should().BeFalse();
            }


            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public void ShouldBeTrueRegardlessOfWhetherOrNotAProjectWithTheSameNameExistsInTheSameWorkspace(bool configureForSameWorkspace)
            {
                saveEnabledObserver = TestScheduler.CreateObserver<bool>();
                SetupDataSourceToReturnExistingProjectsAndDefaultWorkspace(dataSourceProjectIsInSameWorkspace: configureForSameWorkspace);
                var viewModel = CreateViewModel();
                viewModel.Save.Enabled.Subscribe(saveEnabledObserver);
                TestScheduler.Start();

                viewModel.Name.Accept(ProjectName);
                TestScheduler.Start();

                saveEnabledObserver.LastEmittedValue().Should().Be(true);
            }

            [Theory, LogIfTooSlow]
            [InlineData("NotUsedProject")]
            [InlineData("Project-1-1")]
            [InlineData("Project-0-2")]
            [InlineData("Project")]
            public void ShouldAlwaysReturnTrueEvenWhenWorkspaceChanges(string projectName)
            {
                saveEnabledObserver = TestScheduler.CreateObserver<bool>();
                SetupDataSourceToReturnMultipleWorkspaces();
                var viewModel = CreateViewModel();
                viewModel.Save.Enabled.Subscribe(saveEnabledObserver);
                TestScheduler.Start();

                viewModel.Name.Accept(projectName);
                TestScheduler.Start();
                viewModel.PickWorkspace.Execute();
                TestScheduler.Start();

                saveEnabledObserver.LastEmittedValue().Should().Be(true);
            }
        }

        public sealed class TheWorkspace : EditProjectViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task SetsTheWorkspace()
            {
                InteractorFactory
                    .GetDefaultWorkspace()
                    .Execute()
                    .Returns(Observable.Return(Workspace));

                Workspace.Id.Returns(DefaultWorkspaceId);
                Workspace.Name.Returns(DefaultWorkspaceName);

                var viewModel = CreateViewModel();
                await viewModel.Initialize("Some name");

                viewModel.Save.Execute();
                TestScheduler.Start();

                await InteractorFactory
                    .Received()
                    .CreateProject(Arg.Is<CreateProjectDTO>(dto => dto.WorkspaceId == DefaultWorkspaceId))
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public void IsSetToTheFirstEligibleForProjectCreationIfDefaultIsNotEligible()
            {
                var observer = TestScheduler.CreateObserver<string>();
                var defaultWorkspace = Substitute.For<IThreadSafeWorkspace>();
                defaultWorkspace.Name.Returns(DefaultWorkspaceName);
                defaultWorkspace.Admin.Returns(false);
                defaultWorkspace.OnlyAdminsMayCreateProjects.Returns(true);
                var eligibleWorkspace = Substitute.For<IThreadSafeWorkspace>();
                eligibleWorkspace.Name.Returns("Eligible workspace for project creation");
                eligibleWorkspace.Admin.Returns(true);
                InteractorFactory.GetDefaultWorkspace().Execute()
                    .Returns(Observable.Return(defaultWorkspace));
                InteractorFactory.GetAllWorkspaces().Execute()
                    .Returns(Observable.Return(new[] { defaultWorkspace, eligibleWorkspace }));

                var viewModel = CreateViewModel();
                viewModel.WorkspaceName.Subscribe(observer);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(eligibleWorkspace.Name);
            }

            [Fact, LogIfTooSlow]
            public void IsSetToTheDefaultWorkspaceIfAllWorkspacesAreEligibleForProjectCreation()
            {
                var observer = TestScheduler.CreateObserver<string>();
                var defaultWorkspace = Substitute.For<IThreadSafeWorkspace>();
                defaultWorkspace.Name.Returns(DefaultWorkspaceName);
                defaultWorkspace.Admin.Returns(true);
                var eligibleWorkspace = Substitute.For<IThreadSafeWorkspace>();
                eligibleWorkspace.Name.Returns("Eligible workspace for project creation");
                eligibleWorkspace.Admin.Returns(true);
                var eligibleWorkspace2 = Substitute.For<IThreadSafeWorkspace>();
                eligibleWorkspace.Name.Returns("Another Eligible Workspace");
                eligibleWorkspace.Admin.Returns(true);
                InteractorFactory.GetDefaultWorkspace().Execute()
                    .Returns(Observable.Return(defaultWorkspace));
                InteractorFactory.GetAllWorkspaces().Execute()
                    .Returns(Observable.Return(new[] { eligibleWorkspace2, defaultWorkspace, eligibleWorkspace }));

                var viewModel = CreateViewModel();
                TestScheduler.Start();
                viewModel.WorkspaceName.Subscribe(observer);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(defaultWorkspace.Name);
            }
        }

        public sealed class TheIsPrivateProperty : EditProjectViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void DefaultsToTrue()
            {
                ViewModel.IsPrivate.Value.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task IsSetToTrueWhenTheUserSelectsAWorkspaceInWhichTheyAreNotAdmins()
            {
                var mockWorkspace = new MockWorkspace { Admin = false };
                var interactor = Substitute.For<IInteractor<IObservable<IThreadSafeWorkspace>>>();
                interactor.Execute().Returns(Observable.Return(mockWorkspace));
                InteractorFactory.GetWorkspaceById(Arg.Is(1L)).Returns(interactor);
                
                View.Select(
                        Arg.Any<string>(),
                        Arg.Any<IEnumerable<SelectOption<IThreadSafeWorkspace>>>(),
                        Arg.Any<int>())
                    .Returns(Observable.Return(new MockWorkspace { Id = 1L }));
                ViewModel.CanCreatePublicProjects.Subscribe();

                ViewModel.IsPrivate.Accept(false);
                var awaitable = ViewModel.PickWorkspace.ExecuteWithCompletion(Unit.Default);
                TestScheduler.Start();
                await awaitable;

                ViewModel.IsPrivate.Value.Should().BeTrue();
            }
        }

        public sealed class TheCloseWithDefaultResultMethod : EditProjectViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModel()
            {
                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                View.Received().Close();
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsNull()
            {
                ViewModel.Initialize("Some name");

                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                (await ViewModel.Result)
                    .Should().Be(null);
            }

            [Fact, LogIfTooSlow]
            public void DoesNotTrySavingTheChanges()
            {
                ViewModel.Initialize("Some name");

                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                InteractorFactory.CreateProject(Arg.Any<CreateProjectDTO>()).DidNotReceive().Execute();
            }
        }

        public sealed class TheDoneAction : EditProjectViewModelTest
        {
            private const long proWorkspaceId = 11;
            private const long projectId = 12;

            private readonly IThreadSafeProject project = Substitute.For<IThreadSafeProject>();

            public TheDoneAction()
            {
                InteractorFactory
                    .AreCustomColorsEnabledForWorkspace(DefaultWorkspaceId)
                    .Execute()
                    .Returns(Observable.Return(false));

                InteractorFactory
                    .AreCustomColorsEnabledForWorkspace(proWorkspaceId)
                    .Execute()
                    .Returns(Observable.Return(true));

                InteractorFactory
                    .GetDefaultWorkspace()
                    .Execute()
                    .Returns(Observable.Return(Workspace));

                InteractorFactory
                    .GetWorkspaceById(Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(Workspace));

                InteractorFactory
                    .CreateProject(Arg.Any<CreateProjectDTO>())
                    .Execute()
                    .Returns(Observable.Return(project));

                project.Id.Returns(projectId);
                Workspace.Id.Returns(proWorkspaceId);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheIdOfTheCreatedProject()
            {
                ViewModel.Initialize("Some name");
                TestScheduler.Start();

                ViewModel.Save.Execute();
                TestScheduler.Start();

                (await ViewModel.Result)
                    .Should().Be(projectId);
            }

            [Fact, LogIfTooSlow]
            public void DoesNotCallCreateIfTheProjectNameIsInvalid()
            {
                ViewModel.Initialize("Some name");
                TestScheduler.Start();
                ViewModel.Name.Accept("");
                TestScheduler.Start();

                ViewModel.Save.Execute();
                TestScheduler.Start();

                InteractorFactory
                    .DidNotReceive()
                    .CreateProject(Arg.Any<CreateProjectDTO>())
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public void DoesNotCloseTheViewModelIfTheProjectNameIsInvalid()
            {
                ViewModel.Initialize("Some name");
                TestScheduler.Start();
                ViewModel.Name.Accept("");
                TestScheduler.Start();

                ViewModel.Save.Execute();
                TestScheduler.Start();

                View.DidNotReceive().Close();
            }

            [Theory, LogIfTooSlow]
            [InlineData("   abcde", "abcde")]
            [InlineData("abcde     ", "abcde")]
            [InlineData("  abcde ", "abcde")]
            [InlineData("abcde  fgh", "abcde  fgh")]
            [InlineData("      abcd\nefgh     ", "abcd\nefgh")]
            public async Task TrimsNameFromTheStartAndTheEndBeforeSaving(string name, string trimmed)
            {
                ViewModel.Initialize(name);

                ViewModel.Save.Execute();

                TestScheduler.Start();
                await InteractorFactory
                    .Received()
                    .CreateProject(Arg.Is<CreateProjectDTO>(dto => dto.Name == trimmed))
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public void ShowEmitErrorWhenThereIsExistingProject()
            {
                SetupDataSourceToReturnExistingProjectsAndDefaultWorkspace(true);

                var viewModel = CreateViewModel();

                var observer = TestScheduler.CreateObserver<Exception>();
                viewModel.Save.Errors.Subscribe(observer);
                TestScheduler.Start();

                viewModel.Name.Accept(ProjectName);
                viewModel.Save.Execute();
                TestScheduler.Start();

                var messages = observer.Messages;
                messages.Should().HaveCount(1);
                messages.Last().Value.Value.Message.Should().Be(Resources.ProjectNameTakenError);
            }

            public sealed class WhenCreatingProjectInAnotherWorkspace : EditProjectViewModelTest
            {
                private const long defaultWorkspaceId = 101;
                private const long selectedWorkspaceId = 102;

                protected override void AdditionalSetup()
                {
                    var defaultWorkspace = Substitute.For<IThreadSafeWorkspace>();
                    defaultWorkspace.Id.Returns(defaultWorkspaceId);
                    var selectedWorkspace = Substitute.For<IThreadSafeWorkspace>();
                    selectedWorkspace.Id.Returns(selectedWorkspaceId);
                    InteractorFactory
                        .GetDefaultWorkspace()
                        .Execute()
                        .Returns(Observable.Return(defaultWorkspace));

                    View.Select(
                        Arg.Any<string>(),
                        Arg.Any<IEnumerable<SelectOption<IThreadSafeWorkspace>>>(),
                        Arg.Any<int>())
                        .Returns(Observable.Return(new MockWorkspace { Id = selectedWorkspaceId }));
                }

                protected override void AdditionalViewModelSetup()
                {
                    TestScheduler.Start();
                    ViewModel.Initialize("Some project");
                    TestScheduler.Start();
                    ViewModel.WorkspaceName.Subscribe();
                    TestScheduler.Start();
                    ViewModel.PickWorkspace.Execute();
                    TestScheduler.Start();
                }

                [Fact, LogIfTooSlow]
                public void AsksUserForConfirmationIfWorkspaceHasChanged()
                {
                    ViewModel.Save.Execute();
                    TestScheduler.Start();

                    View.Received().Confirm(
                        Arg.Is(Resources.WorkspaceChangedAlertTitle),
                        Arg.Is(Resources.WorkspaceChangedAlertMessage),
                        Arg.Is(Resources.Ok),
                        Arg.Is(Resources.Cancel)
                    );
                }

                [Fact, LogIfTooSlow]
                public void DoesNothingIfUserCancels()
                {
                    View
                        .Confirm(
                            Arg.Is(Resources.WorkspaceChangedAlertTitle),
                            Arg.Is(Resources.WorkspaceChangedAlertMessage),
                            Arg.Is(Resources.Ok),
                            Arg.Is(Resources.Cancel))
                        .Returns(Observable.Return(false));

                    ViewModel.Save.Execute();
                    TestScheduler.Start();

                    InteractorFactory.CreateProject(Arg.Any<CreateProjectDTO>()).DidNotReceive().Execute();
                    View.DidNotReceive().Close();
                }
            }
        }

        public sealed class ThePickColorAction : EditProjectViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void CallsTheSelectColorViewModel()
            {
                ViewModel.Initialize("Some name");

                ViewModel.PickColor.Execute();
                TestScheduler.Start();

                NavigationService.Received()
                    .Navigate<SelectColorViewModel, ColorParameters, Color>(Arg.Any<ColorParameters>(), ViewModel.View);
            }

            [Fact, LogIfTooSlow]
            public void SetsTheReturnedColorAsTheColorProperty()
            {
                var colorObserver = TestScheduler.CreateObserver<Color>();
                var expectedColor = new Color(23, 45, 125);
                NavigationService
                    .Navigate<SelectColorViewModel, ColorParameters, Color>(Arg.Any<ColorParameters>(), ViewModel.View)
                    .Returns(Task.FromResult(expectedColor));
                ViewModel.Color.Subscribe(colorObserver);

                ViewModel.PickColor.Execute();
                TestScheduler.Start();

                colorObserver.LastEmittedValue().Should().Be(expectedColor);
            }
        }

        public sealed class ThePickWorkspaceAction : EditProjectViewModelTest
        {
            private const long workspaceId = 10;
            private const long defaultWorkspaceId = 11;
            private const string workspaceName = "My custom workspace";
            private readonly IThreadSafeWorkspace workspace = Substitute.For<IThreadSafeWorkspace>();
            private readonly IThreadSafeWorkspace defaultWorkspace = Substitute.For<IThreadSafeWorkspace>();

            public ThePickWorkspaceAction()
            {
                workspace.Id.Returns(workspaceId);
                workspace.Name.Returns(workspaceName);
                defaultWorkspace.Id.Returns(defaultWorkspaceId);

                InteractorFactory
                    .GetDefaultWorkspace()
                    .Execute()
                    .Returns(Observable.Return(defaultWorkspace));

                InteractorFactory
                    .GetWorkspaceById(workspaceId)
                    .Execute()
                    .Returns(Observable.Return(workspace));

                ViewModel.Initialize("");
            }

            [Fact, LogIfTooSlow]
            public void CallsTheModalForSelectingWorkspace()
            {
                ViewModel.PickWorkspace.Execute();
                TestScheduler.Start();

                View.Received().Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<IThreadSafeWorkspace>>>(),
                    Arg.Any<int>());
            }

            [Fact, LogIfTooSlow]
            public void SetsTheReturnedWorkspaceNameAsTheWorkspaceNameProperty()
            {
                View.Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<IThreadSafeWorkspace>>>(),
                    Arg.Any<int>())
                    .Returns(Observable.Return(new MockWorkspace { Id = workspaceId }));
                TestScheduler.Start();
                var workspaceObserver = TestScheduler.CreateObserver<string>();
                ViewModel.WorkspaceName.Subscribe(workspaceObserver);

                ViewModel.PickWorkspace.Execute();
                TestScheduler.Start();

                workspaceObserver.LastEmittedValue().Should().Be(workspaceName);
            }

            [Fact, LogIfTooSlow]
            public void ResetsTheClientNameWhenTheWorkspaceChanges()
            {
                View.Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<IThreadSafeWorkspace>>>(),
                    Arg.Any<int>())
                    .Returns(Observable.Return(new MockWorkspace { Id = workspaceId }));
                var clientObserver = TestScheduler.CreateObserver<string>();
                ViewModel.ClientName.Subscribe(clientObserver);

                ViewModel.PickWorkspace.Execute();
                TestScheduler.Start();

                clientObserver.LastEmittedValue().Should().BeNullOrEmpty();
            }

            [Fact, LogIfTooSlow]
            public void PicksADefaultColorIfTheSelectedColorIsCustomAndTheWorkspaceIsNotPro()
            {
                var someColor = new Color(23, 45, 125);
                NavigationService
                    .Navigate<SelectColorViewModel, ColorParameters, Color>(Arg.Any<ColorParameters>(), ViewModel.View)
                    .Returns(Task.FromResult(someColor));
                View.Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<IThreadSafeWorkspace>>>(),
                    Arg.Any<int>())
                    .Returns(Observable.Return(new MockWorkspace { Id = workspaceId }));
                InteractorFactory.AreCustomColorsEnabledForWorkspace(workspaceId).Execute()
                    .Returns(Observable.Return(false));
                ViewModel.PickColor.Execute();
                TestScheduler.Start();

                ViewModel.PickWorkspace.Execute();
                TestScheduler.Start();

                ViewModel.Color.Should().NotBe(someColor);
            }
        }

        public sealed class ThePickClientAction : EditProjectViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void CallsTheSelectClientViewModel()
            {
                ViewModel.Initialize("Some name");

                ViewModel.PickClient.Execute();
                TestScheduler.Start();

                NavigationService.Received()
                    .Navigate<SelectClientViewModel, SelectClientParameters, long?>(Arg.Any<SelectClientParameters>(), ViewModel.View);
            }

            [Fact, LogIfTooSlow]
            public void PassesTheCurrentWorkspaceToTheViewModel()
            {
                InteractorFactory
                    .GetDefaultWorkspace()
                    .Execute()
                    .Returns(Observable.Return(Workspace));
                Workspace.Id.Returns(DefaultWorkspaceId);
                var viewModel = CreateViewModel();
                viewModel.Initialize("Some name");
                viewModel.AttachView(View);
                TestScheduler.Start();

                viewModel.PickClient.Execute();
                TestScheduler.Start();

                NavigationService.Received()
                    .Navigate<SelectClientViewModel, SelectClientParameters, long?>(
                        Arg.Is<SelectClientParameters>(parameter => parameter.WorkspaceId == DefaultWorkspaceId),
                        viewModel.View
                    );
            }

            [Fact, LogIfTooSlow]
            public void SetsTheReturnedClientAsTheClientNameProperty()
            {
                var clientObserver = TestScheduler.CreateObserver<string>();
                const string expectedName = "Some client";
                long? expectedId = 10;
                var client = Substitute.For<IThreadSafeClient>();
                client.Id.Returns(expectedId.Value);
                client.Name.Returns(expectedName);
                NavigationService
                    .Navigate<SelectClientViewModel, SelectClientParameters, long?>(Arg.Any<SelectClientParameters>(), ViewModel.View)
                    .Returns(Task.FromResult(expectedId));
                InteractorFactory
                    .GetDefaultWorkspace()
                    .Execute()
                    .Returns(Observable.Return(Workspace));
                InteractorFactory.GetClientById(expectedId.Value)
                    .Execute()
                    .Returns(Observable.Return(client));
                Workspace.Id.Returns(DefaultWorkspaceId);
                ViewModel.Initialize("Some name");
                ViewModel.ClientName.Subscribe(clientObserver);

                ViewModel.PickClient.Execute();
                TestScheduler.Start();

                clientObserver.LastEmittedValue().Should().Be(expectedName);
            }

            [Fact, LogIfTooSlow]
            public void ClearsTheCurrentClientIfZeroIsReturned()
            {
                var clientObserver = TestScheduler.CreateObserver<string>();
                const string expectedName = "Some client";
                long? expectedId = 10;
                var client = Substitute.For<IThreadSafeClient>();
                client.Id.Returns(expectedId.Value);
                client.Name.Returns(expectedName);
                NavigationService
                    .Navigate<SelectClientViewModel, SelectClientParameters, long?>(Arg.Any<SelectClientParameters>(), ViewModel.View)
                    .Returns(Task.FromResult(expectedId), Task.FromResult<long?>(0));
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(Workspace));
                InteractorFactory.GetClientById(expectedId.Value).Execute().Returns(Observable.Return(client));
                Workspace.Id.Returns(DefaultWorkspaceId);
                ViewModel.Initialize("Some name");
                ViewModel.PickClient.Execute();
                TestScheduler.Start();
                ViewModel.ClientName.Subscribe(clientObserver);

                ViewModel.PickClient.Execute();
                TestScheduler.Start();

                clientObserver.LastEmittedValue().Should().BeNullOrEmpty();
            }
        }

        public sealed class TheErrorProperty : EditProjectViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void EmitsWhenNameChanged()
            {
                var observer = TestScheduler.CreateObserver<string>();
                ViewModel.Error.Subscribe(observer);

                TestScheduler.Start();

                ViewModel.Name.Accept("new name");

                observer.Messages.Last().Value.Value.Should().BeNullOrEmpty();
            }
        }
    }
}
