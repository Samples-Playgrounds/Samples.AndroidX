using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Core.Extensions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class SelectProjectViewModelTests
    {
        public abstract class SelectProjectViewModelTest : BaseViewModelTests<SelectProjectViewModel, SelectProjectParameter, SelectProjectParameter>
        {
            protected SelectProjectParameter DefaultParameter { get; } = new SelectProjectParameter(null, null, 1);

            protected override SelectProjectViewModel CreateViewModel()
            => new SelectProjectViewModel(DataSource, RxActionFactory, InteractorFactory, NavigationService, SchedulerProvider);
        }

        public sealed class TheConstructor : SelectProjectViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useDataSource,
                bool useRxActionFactory,
                bool useInteractorFactory,
                bool useNavigationService,
                bool useSchedulerProvider)
            {
                var dataSource = useDataSource ? DataSource : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
              
                Action tryingToConstructWithEmptyParameters =
                    () => new SelectProjectViewModel(dataSource, rxActionFactory, interactorFactory, navigationService, schedulerProvider);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheCloseWithDefaultResultMethod : SelectProjectViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ClosesTheViewModel()
            {
                ViewModel.CloseWithDefaultResult();

                View.Received().Close();
            }

            [Theory]
            [InlineData(null)]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(124235)]
            public async Task ReturnsTheSameProjectIdThatWasPassedToTheViewModel(long? projectId)
            {
                await ViewModel.Initialize(new SelectProjectParameter(projectId, 10, 11));

                ViewModel.CloseWithDefaultResult();

                (await ViewModel.Result)
                    .ProjectId.Should().Be(projectId);
            }

            [Theory]
            [InlineData(null)]
            [InlineData(0)]
            [InlineData(1)]
            [InlineData(124235)]
            public async Task ReturnsTheSameTaskIdThatWasPassedToTheViewModel(long? taskId)
            {
                await ViewModel.Initialize(new SelectProjectParameter(10, taskId, 11));

                ViewModel.CloseWithDefaultResult();

                (await ViewModel.Result)
                    .TaskId.Should().Be(taskId);
            }
        }

        public sealed class TheSelectProjectCommand : SelectProjectViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModel()
            {
                ViewModel.SelectProject
                    .Execute(ProjectSuggestion.NoProject(0, ""));

                View.Received().Close();
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheSelectedProjectIdWhenSelectingAProject()
            {
                var project = Substitute.For<IThreadSafeProject>();
                project.Id.Returns(13);
                var selectedProject = new ProjectSuggestion(project);

                ViewModel.SelectProject.Execute(selectedProject);

                (await ViewModel.Result)
                    .ProjectId.Should().Be(selectedProject.ProjectId);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsNoTaskIdWhenSelectingAProject()
            {
                var project = Substitute.For<IThreadSafeProject>();
                project.Id.Returns(13);
                var selectedProject = new ProjectSuggestion(project);

                ViewModel.SelectProject.Execute(selectedProject);

                (await ViewModel.Result)
                    .TaskId.Should().Be(null);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheSelectedProjectIdWhenSelectingATask()
            {
                var task = Substitute.For<IThreadSafeTask>();
                task.Id.Returns(13);
                task.ProjectId.Returns(10);
                var selectedTask = new TaskSuggestion(task);

                ViewModel.SelectProject.Execute(selectedTask);

                (await ViewModel.Result)
                    .ProjectId.Should().Be(task.ProjectId);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheSelectedTaskIdWhenSelectingATask()
            {
                var task = Substitute.For<IThreadSafeTask>();
                task.Id.Returns(13);
                task.ProjectId.Returns(10);
                var selectedTask = new TaskSuggestion(task);

                ViewModel.SelectProject.Execute(selectedTask);

                (await ViewModel.Result)
                    .ProjectId.Should().Be(task.ProjectId);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsNoProjectIfNoProjectWasSelected()
            {
                ViewModel.SelectProject
                    .Execute(ProjectSuggestion.NoProject(0, ""));

                (await ViewModel.Result)
                    .ProjectId.Should().Be(null);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsNoTaskIfNoProjectWasSelected()
            {
                ViewModel.SelectProject
                    .Execute(ProjectSuggestion.NoProject(0, ""));

                (await ViewModel.Result)
                    .TaskId.Should().Be(null);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsWorkspaceIfNoProjectWasSelected()
            {
                View.Confirm(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>()
                ).Returns(Observable.Return(true));

                long workspaceId = 420;
                ViewModel.SelectProject
                    .Execute(ProjectSuggestion.NoProject(workspaceId, ""));

                (await ViewModel.Result)
                    .WorkspaceId.Should().Be(workspaceId);
            }

            [Fact, LogIfTooSlow]
            public void ShowsAlertIfWorkspaceIsGoingToBeChanged()
            {
                var oldWorkspaceId = 10;
                var newWorkspaceId = 11;

                ViewModel.Initialize(new SelectProjectParameter(null, null, oldWorkspaceId));

                var project = Substitute.For<IThreadSafeProject>();
                project.WorkspaceId.Returns(newWorkspaceId);

                ViewModel.SelectProject.Execute(new ProjectSuggestion(project));

                View.Received().Confirm(
                    Arg.Is(Resources.DifferentWorkspaceAlertTitle),
                    Arg.Is(Resources.DifferentWorkspaceAlertMessage),
                    Arg.Is(Resources.Ok),
                    Arg.Is(Resources.Cancel)
                );
            }

            [Fact, LogIfTooSlow]
            public void DoesNotShowsAlertIfWorkspaceIsNotGoingToBeChanged()
            {
                var workspaceId = 10;

                ViewModel.Initialize(new SelectProjectParameter(null, null, workspaceId));

                var project = Substitute.For<IThreadSafeProject>();
                project.WorkspaceId.Returns(workspaceId);

                ViewModel.SelectProject.Execute(new ProjectSuggestion(project));

                View.DidNotReceive().Confirm(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>()
                );
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsWorkspaceIdOfTheProjectIfProjectWasSelected()
            {
                var project = Substitute.For<IThreadSafeProject>();
                project.WorkspaceId.Returns(13);
                var projectSuggestion = new ProjectSuggestion(project);
                prepareDialoag();

                ViewModel.SelectProject.Execute(projectSuggestion);

                await ensureReturnsWorkspaceIdOfSuggestion(projectSuggestion);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsWorksaceIdIfNoProjectWasSelected()
            {
                var noProjectSuggestion = ProjectSuggestion.NoProject(13, "");
                prepareDialoag();

                ViewModel.SelectProject.Execute(noProjectSuggestion);

                await ensureReturnsWorkspaceIdOfSuggestion(noProjectSuggestion);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsWorkspaceIdOfTheTaskIfTaskWasSelected()
            {
                var task = Substitute.For<IThreadSafeTask>();
                task.Id.Returns(13);
                var taskSuggestion = new TaskSuggestion(task);
                prepareDialoag();

                ViewModel.SelectProject.Execute(taskSuggestion);

                await ensureReturnsWorkspaceIdOfSuggestion(taskSuggestion);
            }

            private void prepareDialoag()
                => View.Confirm(
                       Resources.DifferentWorkspaceAlertTitle,
                       Resources.DifferentWorkspaceAlertMessage,
                       Resources.Ok,
                       Resources.Cancel).Returns(Observable.Return(true));

            private async Task ensureReturnsWorkspaceIdOfSuggestion(AutocompleteSuggestion suggestion)
            {
                (await ViewModel.Result)
                    .WorkspaceId.Should().Be(suggestion.WorkspaceId);
            }

            public sealed class WhenTheSuggestionIsCreateEntitySuggestion : SelectProjectViewModelTest
            {
                [Fact, LogIfTooSlow]
                public async Task NavigatesToEditProjectViewModel()
                {
                    var projectName = "Some project";
                    var createEntitySuggestion = new CreateEntitySuggestion(Resources.CreateProject, projectName);

                    await ViewModel.Initialize(new SelectProjectParameter(null, null, 10));

                    ViewModel.SelectProject.Execute(createEntitySuggestion);
                    TestScheduler.Start();

                    await NavigationService.Received().Navigate<EditProjectViewModel, string, long?>(projectName, View);
                }

                [Fact, LogIfTooSlow]
                public async Task DoesNotCloseTheViewModelIfTheProjectIsNotCreated()
                {
                    var projectName = "New project name";
                    var createProjectSuggestion = new CreateEntitySuggestion(Resources.CreateProject, projectName);
                    setupProjectCreationResult(null);

                    await ViewModel.Initialize(new SelectProjectParameter(null, null, 10));

                    TestScheduler.Start();

                    ViewModel.SelectProject.Execute(createProjectSuggestion);
                    TestScheduler.Start();

                    View.DidNotReceive().Close();
                }

                [Fact, LogIfTooSlow]
                public async Task ClosesTheViewModelReturningTheCreatedIdIfTheProjectIsCreated()
                {
                    var workspace = new MockWorkspace { Id = 1, Name = "ws", Admin = true, OnlyAdminsMayCreateProjects = true };
                    InteractorFactory.GetAllWorkspaces().Execute().Returns(Observable.Return(new[] { workspace }));
                    const long projectId = 10;
                    setupProjectCreationResult(projectId);

                    await ViewModel.Initialize(new SelectProjectParameter(null, null, 10));

                    var projectName = "Some Project";
                    var createProjectSuggestion = new CreateEntitySuggestion(Resources.CreateProject, projectName);

                    ViewModel.SelectProject.Execute(createProjectSuggestion);
                    TestScheduler.Start();

                    (await ViewModel.Result)
                        .ProjectId.Should().Be(projectId);
                }

                private void setupProjectCreationResult(long? returnedId)
                {
                    NavigationService
                        .Navigate<EditProjectViewModel, string, long?>(Arg.Any<string>(), ViewModel.View)
                        .Returns(Task.FromResult(returnedId));

                    if (returnedId == null) return;

                    var project = Substitute.For<IThreadSafeProject>();
                    project.Id.Returns(returnedId.Value);
                    InteractorFactory.GetProjectById(returnedId.Value).Execute().Returns(Observable.Return(project));
                }
            }
        }

        public sealed class TheTextProperty : SelectProjectViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task WhenChangedUsesTheGetProjectsAutocompleteSuggestionsInteractor()
            {
                var text = "Some text";
                await ViewModel.Initialize(DefaultParameter);

                TestScheduler.Start();
                ViewModel.FilterText.OnNext(text);
                TestScheduler.AdvanceBy(TimeSpan.FromSeconds(60).Ticks);

                InteractorFactory
                    .Received()
                    .GetProjectsAutocompleteSuggestions(Arg.Is<IList<string>>(
                        words => words.SequenceEqual(text.SplitToQueryWords())));
            }
        }

        public sealed class TheSuggestionsProperty : SelectProjectViewModelTest
        {
            private IEnumerable<ProjectSuggestion> getProjectSuggestions(int count, int workspaceId)
            {
                for (int i = 0; i < count; i++)
                    yield return getProjectSuggestion(i, workspaceId);
            }

            private ProjectSuggestion getProjectSuggestion(int projectId, int workspaceId)
            {
                return getProjectSuggestion(projectId, workspaceId, new List<IThreadSafeTask>());
            }

            private ProjectSuggestion getProjectSuggestion(int projectId, int workspaceId, IEnumerable<IThreadSafeTask> tasks)
            {
                var workspace = new MockWorkspace
                {
                    Name = $"Workspace{workspaceId}",
                    Id = workspaceId
                };
                var project = new MockProject
                {
                    Name = $"Project{projectId}",
                    Workspace = workspace,
                    WorkspaceId = workspaceId,
                    Active = true,
                    Tasks = tasks
                };
                return new ProjectSuggestion(project);
            }

            private IThreadSafeWorkspace setupWorkspace(int id, bool isEligibleForProjectCreation)
            {
                var workspace = new MockWorkspace { Id = id, Name = "ws", Admin = false, OnlyAdminsMayCreateProjects = !isEligibleForProjectCreation };
                InteractorFactory.GetAllWorkspaces().Execute().Returns(Observable.Return(new[] { workspace }));
                return workspace;
            }

            [Fact, LogIfTooSlow]
            public async Task StartsWithAnEmptyList()
            {
                var observer = TestScheduler.CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);
                TestScheduler.Start();

                observer.Messages.First().Value.Value.Should().HaveCount(0);
            }

            [Fact, LogIfTooSlow]
            public async Task IsPopulatedAfterInitialization()
            {
                var workspaceId = 0;
                var projectSuggestions = getProjectSuggestions(10, workspaceId);
                var suggestionsObservable = Observable.Return(projectSuggestions);
                InteractorFactory
                    .GetProjectsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(suggestionsObservable);
                var observer = TestScheduler.CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(2);
                observer.Messages[1].Value.Value.Should().HaveCount(1);
                observer.Messages[1].Value.Value.First().Items.Should().HaveCount(11);
            }

            [Fact, LogIfTooSlow]
            public async Task PrependsEmptyProjectToEveryGroupIfFilterIsEmpty()
            {
                var suggestions = new List<ProjectSuggestion>();
                suggestions.AddRange(getProjectSuggestions(3, workspaceId: 0));
                suggestions.AddRange(getProjectSuggestions(4, workspaceId: 1));
                suggestions.AddRange(getProjectSuggestions(1, workspaceId: 10));
                suggestions.AddRange(getProjectSuggestions(10, workspaceId: 54));
                var suggestionsObservable = Observable.Return(suggestions);
                InteractorFactory
                    .GetProjectsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(suggestionsObservable);
                var observer = TestScheduler.CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);
                TestScheduler.Start();

                foreach (var section in observer.Messages.Last().Value.Value)
                    section.Items.Cast<ProjectSuggestion>().First().ProjectName.Should().Be(Resources.NoProject);
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotPrependEmptyProjectToGroupsIfFilterIsUsed()
            {
                var suggestions = new List<ProjectSuggestion>();
                suggestions.AddRange(getProjectSuggestions(3, workspaceId: 0));
                suggestions.AddRange(getProjectSuggestions(4, workspaceId: 1));
                suggestions.AddRange(getProjectSuggestions(1, workspaceId: 10));
                suggestions.AddRange(getProjectSuggestions(10, workspaceId: 54));
                var suggestionsObservable = Observable.Return(suggestions);
                InteractorFactory
                    .GetProjectsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(suggestionsObservable);
                var observer = TestScheduler.CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);
                ViewModel.FilterText.OnNext(suggestions.First().ProjectName);
                TestScheduler.Start();

                foreach (var section in observer.Messages.Last().Value.Value)
                {
                    section.Items.Cast<ProjectSuggestion>().First().ProjectName.Should().NotBe(Resources.NoProject);
                }
            }

            [Fact, LogIfTooSlow]
            public async Task GroupsProjectsByWorkspace()
            {
                var suggestions = new List<ProjectSuggestion>();
                var workspaceIds = new[] { 0, 1, 10, 54 };
                suggestions.AddRange(getProjectSuggestions(3, workspaceId: workspaceIds[0]));
                suggestions.AddRange(getProjectSuggestions(4, workspaceId: workspaceIds[1]));
                suggestions.AddRange(getProjectSuggestions(1, workspaceId: workspaceIds[2]));
                suggestions.AddRange(getProjectSuggestions(10, workspaceId: workspaceIds[3]));
                var suggestionsObservable = Observable.Return(suggestions);
                InteractorFactory
                    .GetProjectsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(suggestionsObservable);
                var observer = TestScheduler.CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);
                TestScheduler.Start();

                var latestSuggestions = observer.Messages.Last().Value.Value.ToArray();
                latestSuggestions.Should().HaveCount(4);

                for (int i = 0; i < latestSuggestions.Length; i++)
                {
                    foreach (var suggestion in latestSuggestions[i].Items)
                    {
                        suggestion.WorkspaceName.Should().Be(latestSuggestions[i].Header);
                        suggestion.WorkspaceId.Should().Be(workspaceIds[i]);
                    }
                }
            }

            [Fact, LogIfTooSlow]
            public async Task SortsProjectsByName()
            {
                var suggestions = new List<ProjectSuggestion>();
                suggestions.Add(getProjectSuggestion(3, 0));
                suggestions.Add(getProjectSuggestion(4, 1));
                suggestions.Add(getProjectSuggestion(1, 0));
                suggestions.Add(getProjectSuggestion(33, 1));
                suggestions.Add(getProjectSuggestion(10, 1));
                var suggestionsObservable = Observable.Return(suggestions);
                InteractorFactory
                    .GetProjectsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(suggestionsObservable);
                var observer = TestScheduler.CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);
                TestScheduler.Start();

                var latestSuggestions = observer.Messages.Last().Value.Value;
                latestSuggestions.Should().HaveCount(2);
                foreach (var section in latestSuggestions)
                {
                    section.Items
                        .Cast<ProjectSuggestion>()
                        .Should()
                        .BeInAscendingOrder(projectSuggestion => projectSuggestion.ProjectName);
                }
            }

            [Fact, LogIfTooSlow]
            public async Task SortsTasksByName()
            {
                var suggestions = new List<ProjectSuggestion>
                {
                    getProjectSuggestion(3, 0, new[]
                    {
                        new MockTask { Id = 1, WorkspaceId = 0, ProjectId = 3, Name = "Task1" },
                        new MockTask { Id = 2, WorkspaceId = 0, ProjectId = 3, Name = "Task2" },
                        new MockTask { Id = 3, WorkspaceId = 0, ProjectId = 3, Name = "Task3" }
                    })
                };

                InteractorFactory.GetProjectsAutocompleteSuggestions(Arg.Any<IList<string>>()).Execute()
                    .Returns(Observable.Return(suggestions));
                var observer = TestScheduler.CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);
                TestScheduler.Start();

                ViewModel.ToggleTaskSuggestions.Execute(suggestions[0]);
                TestScheduler.Start();

                var latestSuggestions = observer.LastEmittedValue();
                latestSuggestions.Should().HaveCount(1);
                latestSuggestions.First().Items.Should().HaveCount(5);
                latestSuggestions.First().Items
                    .Where(suggestion => suggestion is TaskSuggestion)
                    .Cast<TaskSuggestion>()
                    .Should()
                    .BeInAscendingOrder(taskSuggestion => taskSuggestion.Name);
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotContainASelectedProjectIfProjectIdIsNull()
            {
                var suggestions = getProjectSuggestions(20, workspaceId: 10);
                var suggestionsObservable = Observable.Return(suggestions);
                InteractorFactory
                    .GetProjectsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(suggestionsObservable);
                var parameter = new SelectProjectParameter(null, null, 0);
                var observer = TestScheduler.CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(parameter);
                TestScheduler.Start();

                var latestSugestions = observer.Messages.Last().Value.Value;
                latestSugestions.Should().HaveCount(1);
                latestSugestions.First().Items
                    .Cast<ProjectSuggestion>()
                    .Should().OnlyContain(suggestion => !suggestion.Selected);
            }

            [Fact, LogIfTooSlow]
            public async Task ContainsOnlyOneSelectedProjectIfProjectIdIsSet()
            {
                var suggestions = getProjectSuggestions(20, workspaceId: 10);
                var suggestionsObservable = Observable.Return(suggestions);
                InteractorFactory
                    .GetProjectsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(suggestionsObservable);
                long selectedProjectId = 5;
                var parameter = new SelectProjectParameter(selectedProjectId, null, 0);
                var observer = TestScheduler.CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(parameter);
                TestScheduler.Start();

                var latestSuggestions = observer.Messages.Last().Value.Value;
                latestSuggestions.Should().HaveCount(1);
                latestSuggestions.First().Items.Should()
                    .OnlyContain(suggestion => assertSuggestion(suggestion, selectedProjectId));
            }

            private bool assertSuggestion(AutocompleteSuggestion suggestion, long selectedProjectId)
            {
                var projectSuggestion = (ProjectSuggestion)suggestion;
                return projectSuggestion.Selected == false
                       || projectSuggestion.ProjectId == selectedProjectId && projectSuggestion.Selected;
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotIncludeProjectCreationRowWhenTheTextIsEmpty()
            {
                var workspaceId = 0;
                setupWorkspace(workspaceId, isEligibleForProjectCreation: true);
                var projectSuggestions = getProjectSuggestions(10, workspaceId: workspaceId);
                var suggestionsObservable = Observable.Return(projectSuggestions);
                InteractorFactory
                    .GetProjectsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(suggestionsObservable);
                var observer = TestScheduler.CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);
                TestScheduler.Start();

                var latestSuggestions = observer.Messages.Last().Value.Value;
                latestSuggestions.Should().HaveCount(1);
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotIncludeProjectCreationRowWhenTheTextIsWhitespace()
            {
                var workspaceId = 0;
                setupWorkspace(workspaceId, isEligibleForProjectCreation: true);
                var projectSuggestions = getProjectSuggestions(10, workspaceId);
                var suggestionsObservable = Observable.Return(projectSuggestions);
                InteractorFactory
                    .GetProjectsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(suggestionsObservable);
                var observer = TestScheduler.CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);
                ViewModel.FilterText.OnNext("  \t   \t  ");
                TestScheduler.Start();

                var latestSuggestions = observer.Messages.Last().Value.Value;
                latestSuggestions.Should().HaveCount(1);
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotIncludeProjectCreationRowWhenTheTextIsLongerThanTwoHundredAndFiftyCharacters()
            {
                var workspaceId = 0;
                setupWorkspace(workspaceId, isEligibleForProjectCreation: true);
                var projectSuggestions = getProjectSuggestions(10, workspaceId: 0);
                var suggestionsObservable = Observable.Return(projectSuggestions);
                InteractorFactory
                    .GetProjectsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(suggestionsObservable);
                var observer = TestScheduler.CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);
                ViewModel.FilterText.OnNext("Some absurdly long project name created solely for making sure that the SuggestCreation property returns false when the project name is longer than the previously specified threshold so that the mobile apps behave and avoid crashes in backend and even bigger problems.");
                TestScheduler.Start();

                var latestSuggestions = observer.Messages.Last().Value.Value;
                latestSuggestions.Should().HaveCount(1);
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotIncludeProjectCreationRowWhenNoWorkspaceIsEligible()
            {
                var workspaceId = 0;
                setupWorkspace(workspaceId, false);
                var projectSuggestions = getProjectSuggestions(10, workspaceId: 1);
                var suggestionsObservable = Observable.Return(projectSuggestions);
                InteractorFactory
                    .GetProjectsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(suggestionsObservable);
                var observer = TestScheduler.CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);
                ViewModel.FilterText.OnNext("This filter text should result in project creation");
                TestScheduler.Start();

                var latestSuggestions = observer.Messages.Last().Value.Value;
                latestSuggestions.Should().HaveCount(1);
            }

            [Fact, LogIfTooSlow]
            public async Task IncludesProjecCreationWhenFilterTextDoesNotMatchAnySuggestion()
            {
                var workspaceId = 1;
                setupWorkspace(workspaceId, isEligibleForProjectCreation: true);
                var projectSuggestions = getProjectSuggestions(10, workspaceId);
                var suggestionsObservable = Observable.Return(projectSuggestions);
                InteractorFactory
                    .GetProjectsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(suggestionsObservable);
                var observer = TestScheduler.CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);
                var filterText = "Project1a";
                ViewModel.FilterText.OnNext(filterText);
                TestScheduler.Start();

                var latestSuggestions = observer.Messages.Last().Value.Value;
                latestSuggestions.Should().HaveCount(2);
                latestSuggestions.First().Header.Should().BeNull();
                latestSuggestions.First().Items.Should().HaveCount(1);
                var createEntitySuggestion = (CreateEntitySuggestion)latestSuggestions.First().Items.First();
                createEntitySuggestion.EntityName.Should().Be(filterText);
            }

            [Fact, LogIfTooSlow]
            public async Task IncludesProjecCreationWhenFilterTextMatchesASuggestion()
            {
                var workspaceId = 0;
                setupWorkspace(workspaceId, isEligibleForProjectCreation: true);
                var projectSuggestions = getProjectSuggestions(10, workspaceId: 1);
                var suggestionsObservable = Observable.Return(projectSuggestions);
                var filterText = projectSuggestions.First().ProjectName;
                InteractorFactory
                    .GetProjectsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(suggestionsObservable);
                var observer = TestScheduler.CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);
                ViewModel.FilterText.OnNext(filterText);
                TestScheduler.Start();

                var latestSuggestions = observer.Messages.Last().Value.Value;
                latestSuggestions.Should().HaveCount(2);
                latestSuggestions.First().Header.Should().BeNull();
                latestSuggestions.First().Items.Should().HaveCount(1);
                var createEntitySuggestion = (CreateEntitySuggestion)latestSuggestions.First().Items.First();
                createEntitySuggestion.EntityName.Should().Be(filterText);
            }
        }

        public sealed class TheIsEmptyProperty : SelectProjectViewModelTest
        {
            private const long workspaceId = 1;

            private IThreadSafeProject createArbitraryProject(int id)
                => new MockProject
                {
                    Id = id,
                    WorkspaceId = workspaceId,
                    Name = Guid.NewGuid().ToString()
                };

            [Fact, LogIfTooSlow]
            public async Task EmitsTrueIfThereAreNoProjectsInTheDataSource()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsEmpty.Subscribe(observer);

                await ViewModel.Initialize(new SelectProjectParameter(null, null, workspaceId));

                TestScheduler.Start();

                observer.Messages.First().Value.Value.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task EmitsFalseIfThereAreProjectsInTheDataSource()
            {
                var project = createArbitraryProject(10);
                var projectsObservable = Observable.Return(new[] { project });
                DataSource.Projects.GetAll().Returns(projectsObservable);
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsEmpty.Subscribe(observer);

                await ViewModel.Initialize(new SelectProjectParameter(null, null, workspaceId));

                TestScheduler.Start();

                observer.Messages.First().Value.Value.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task TheObservableCompletesAfterTheFirstValueIsEmited()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsEmpty.Subscribe(observer);

                await ViewModel.Initialize(new SelectProjectParameter(null, null, workspaceId));

                TestScheduler.Start();

                observer.Messages.Should().HaveCount(2);
                observer.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
            }
        }
    }
}
