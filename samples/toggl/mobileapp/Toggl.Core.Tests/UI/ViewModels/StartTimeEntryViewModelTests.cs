using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Autocomplete;
using Toggl.Core.Autocomplete.Span;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Interactors.AutocompleteSuggestions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Search;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xunit;
using static Toggl.Core.Helper.Constants;
using CollectionSections = System.Collections.Generic.IEnumerable<Toggl.Core.UI.Collections.SectionModel<string, Toggl.Core.Autocomplete.Suggestions.AutocompleteSuggestion>>;
using ITimeEntryPrototype = Toggl.Core.Models.ITimeEntryPrototype;
using TextFieldInfo = Toggl.Core.Autocomplete.TextFieldInfo;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class StartTimeEntryViewModelTests
    {
        public abstract class StartTimeEntryViewModelTest : BaseViewModelWithInputTests<StartTimeEntryViewModel, StartTimeEntryParameters>
        {
            protected const string TagName = "Mobile";

            protected const long TagId = 20;
            protected const long TaskId = 30;
            protected const long ProjectId = 10;
            protected const long WorkspaceId = 40;
            protected const string WorkspaceName = "The best workspace ever";
            protected const string ProjectName = "Toggl";
            protected const string ProjectColor = "#F41F19";
            protected const string Description = "Testing Toggl mobile apps";

            protected ITestableObserver<TextFieldInfo> TextFieldInfoObserver { get; private set; }

            protected StartTimeEntryParameters DefaultParameter { get; } =
                new StartTimeEntryParameters(DateTimeOffset.UtcNow, "", null, null);

            protected override void AdditionalSetup()
            {
                View.Confirm(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>()
                ).Returns(Observable.Return(true));
            }

            protected override void AdditionalViewModelSetup()
            {
                TextFieldInfoObserver = TestScheduler.CreateObserver<TextFieldInfo>();
                ViewModel.TextFieldInfo.Subscribe(TextFieldInfoObserver);
            }

            protected override StartTimeEntryViewModel CreateViewModel()
                => new StartTimeEntryViewModel(
                    TimeService,
                    DataSource,
                    UserPreferences,
                    OnboardingStorage,
                    InteractorFactory,
                    NavigationService,
                    AnalyticsService,
                    SchedulerProvider,
                    RxActionFactory
                );
        }

        public sealed class TheConstructor : StartTimeEntryViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useDataSource,
                bool useTimeService,
                bool useUserPreferences,
                bool useInteractorFactory,
                bool useOnboardingStorage,
                bool useNavigationService,
                bool useAnalyticsService,
                bool useSchedulerProvider,
                bool useRxActionFactory)
            {
                var dataSource = useDataSource ? DataSource : null;
                var timeService = useTimeService ? TimeService : null;
                var userPreferences = useUserPreferences ? UserPreferences : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var onboardingStorage = useOnboardingStorage ? OnboardingStorage : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var analyticsService = useAnalyticsService ? AnalyticsService : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new StartTimeEntryViewModel(
                        timeService,
                        dataSource,
                        userPreferences,
                        onboardingStorage,
                        interactorFactory,
                        navigationService,
                        analyticsService,
                        schedulerProvider,
                        rxActionFactory);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheInitializeMethod : StartTimeEntryViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public async Task ChecksIfBillableIsAvailableForTheDefaultWorkspace(bool billableValue)
            {
                var workspace = new MockWorkspace { Id = 10 };
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(workspace));
                InteractorFactory.IsBillableAvailableForWorkspace(workspace.Id).Execute()
                    .Returns(Observable.Return(billableValue));
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsBillableAvailable.Subscribe(observer);
                ViewModel.Suggestions.Subscribe();

                var parameter = new StartTimeEntryParameters(DateTimeOffset.UtcNow, "", null, null);
                await ViewModel.Initialize(parameter);

                TestScheduler.Start();
                observer.LastEmittedValue().Should().Be(billableValue);
            }

            [Property]
            public void SetsTheDateAccordingToTheDateParameterReceived(string placeholder)
            {
                var parameter = new StartTimeEntryParameters(DateTimeOffset.Now, placeholder, TimeSpan.Zero, null);
                ViewModel.Initialize(parameter).Wait();

                ViewModel.PlaceholderText.Should().Be(placeholder);
            }

            [Fact, LogIfTooSlow]
            public void StarsTheTimerWhenDurationIsNull()
            {
                var observable = Substitute.For<IConnectableObservable<DateTimeOffset>>();
                TimeService.CurrentDateTimeObservable.Returns(observable);
                var parameter = StartTimeEntryParameters.ForTimerMode(DateTimeOffset.Now, false);

                ViewModel.Initialize(parameter);

                TimeService.CurrentDateTimeObservable.ReceivedWithAnyArgs().Subscribe(null);
            }

            [Fact, LogIfTooSlow]
            public void SetsTheDisplayedTimeToTheValueOfTheDurationParameter()
            {
                var duration = TimeSpan.FromSeconds(130);
                var parameter = new StartTimeEntryParameters(DateTimeOffset.Now, "", duration, null);
                var observer = TestScheduler.CreateObserver<string>();
                ViewModel.DisplayedTime.Subscribe(observer);

                ViewModel.Initialize(parameter);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(duration.ToFormattedString(DurationFormat.Improved));
            }
        }

        public abstract class CreateSuggestionCellModels : StartTimeEntryViewModelTest
        {
            protected abstract int MaxLength { get; }
            protected abstract char QuerySymbol { get; }
            protected abstract string QueryWithExactSuggestionMatch { get; }

            [Fact, LogIfTooSlow]
            public async Task WontExistIfSuggestingTimeEntries()
            {
                var observer = TestScheduler.CreateObserver<CollectionSections>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);

                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan("", 0));

                TestScheduler.Start();
                observer.LastEmittedValue()
                    .Where(firstItemIsCreateSuggestion)
                    .Should().BeEmpty();
            }

            [Fact, LogIfTooSlow]
            public async Task WontExistIfTheCurrentQueryIsEmpty()
            {
                var observer = TestScheduler.CreateObserver<CollectionSections>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);

                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan($"{QuerySymbol}", 1));

                TestScheduler.Start();
                observer.LastEmittedValue()
                    .Where(firstItemIsCreateSuggestion)
                    .Should().BeEmpty();
            }

            [Fact, LogIfTooSlow]
            public async Task WontExistIfTheCurrentQueryIsOnlyWhitespace()
            {
                var observer = TestScheduler.CreateObserver<CollectionSections>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);

                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan($"{QuerySymbol}    ", 1));

                TestScheduler.Start();
                observer.LastEmittedValue()
                    .Where(firstItemIsCreateSuggestion)
                    .Should().BeEmpty();
            }

            [Fact, LogIfTooSlow]
            public async Task WontExistIfTheCurrentQueryIsLongerThanMaxLength()
            {
                var observer = TestScheduler.CreateObserver<CollectionSections>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);

                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan($"{QuerySymbol}{createLongString(MaxLength + 1)}",
                    1));

                TestScheduler.Start();
                observer.LastEmittedValue()
                    .Where(firstItemIsCreateSuggestion)
                    .Should().BeEmpty();
            }

            [Fact, LogIfTooSlow]
            public async Task WontExistIfSuchSuggestionAlreadyExists()
            {
                var observer = TestScheduler.CreateObserver<CollectionSections>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);

                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan($"{QuerySymbol}{QueryWithExactSuggestionMatch}",
                    1));

                TestScheduler.Start();

                observer.LastEmittedValue()
                    .Where(firstItemIsCreateSuggestion)
                    .Should().BeEmpty();
            }

            [Fact, LogIfTooSlow]
            public async Task WontExistIfWorkspaceSettingsDisableProjectCreation()
            {
                var observer = TestScheduler.CreateObserver<CollectionSections>();
                ViewModel.Suggestions.Subscribe(observer);

                var workspace = new MockWorkspace { Id = 1, Admin = false, OnlyAdminsMayCreateProjects = true };
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(workspace));

                await ViewModel.Initialize(DefaultParameter);

                TestScheduler.Start();
                observer.LastEmittedValue()
                    .Where(firstItemIsCreateSuggestion)
                    .Should().BeEmpty();
            }

            private string createLongString(int length)
                => Enumerable
                    .Range(0, length)
                    .Aggregate(new StringBuilder(), (builder, _) => builder.Append('A'))
                    .ToString();

            private bool firstItemIsCreateSuggestion(SectionModel<string, AutocompleteSuggestion> collection)
            {
                return collection.Items.FirstOrDefault() is CreateEntitySuggestion;
            }

            private bool firstItemIsCreateSuggestion(SectionModel<string, AutocompleteSuggestion> collection, string expectedEntityName)
            {
                return collection.Items.FirstOrDefault() is CreateEntitySuggestion createEntitySuggestion
                    && createEntitySuggestion.EntityName == expectedEntityName;
            }

            public sealed class WhenSuggestingProjects : CreateSuggestionCellModels
            {
                protected override int MaxLength => MaxProjectNameLengthInBytes;
                protected override char QuerySymbol => '@';
                protected override string QueryWithExactSuggestionMatch => ProjectName;

                public WhenSuggestingProjects()
                {
                    var project = Substitute.For<IThreadSafeProject>();
                    project.Id.Returns(10);
                    project.Name.Returns(ProjectName);
                    project.WorkspaceId.Returns(40);
                    project.Workspace.Name.Returns("Some workspace");
                    var projectSuggestion = new ProjectSuggestion(project);

                    InteractorFactory
                        .GetAutocompleteSuggestions(
                            Arg.Is<QueryInfo>(info => info.SuggestionType == AutocompleteSuggestionType.Projects),
                            Arg.Any<DefaultTimeEntrySearchEngine>())
                        .Execute()
                        .Returns(Observable.Return(new ProjectSuggestion[] { projectSuggestion }));

                    ViewModel.Initialize(DefaultParameter);
                    ViewModel.Suggestions.Subscribe(); // Otherwise the observable chaing won't trigger
                }

                [Fact, LogIfTooSlow]
                public async Task WontExistIfAProjectIsAlreadySelected()
                {
                    var observer = TestScheduler.CreateObserver<CollectionSections>();
                    ViewModel.Suggestions.Subscribe(observer);

                    var projectSpan = new ProjectSpan(ProjectId, ProjectName, ProjectColor, null, null);
                    var querySpan = new QueryTextSpan("abcde @fgh", 10);

                    ViewModel.OnTextFieldInfoFromView(projectSpan);
                    ViewModel.OnTextFieldInfoFromView(projectSpan, querySpan);

                    TestScheduler.Start();
                    observer.LastEmittedValue()
                        .Where(firstItemIsCreateSuggestion)
                        .Should().BeEmpty();
                }

                [Fact, LogIfTooSlow]
                public async Task WontExistIfAProjectIsAlreadySelectedEvenIfInProjectSelectionMode()
                {
                    var observer = TestScheduler.CreateObserver<CollectionSections>();
                    ViewModel.Suggestions.Subscribe(observer);

                    var projectSpan = new ProjectSpan(ProjectId, ProjectName, ProjectColor, null, null);
                    var querySpan = new QueryTextSpan("abcde @fgh", 10);

                    ViewModel.OnTextFieldInfoFromView(projectSpan);
                    ViewModel.ToggleProjectSuggestions.Execute();

                    ViewModel.OnTextFieldInfoFromView(projectSpan, querySpan);

                    TestScheduler.Start();
                    observer.LastEmittedValue()
                        .Where(firstItemIsCreateSuggestion)
                        .Should().BeEmpty();
                }

                [Fact, LogIfTooSlow]
                public async Task ExistIfTheProjectNameIsValid()
                {
                    var observer = TestScheduler.CreateObserver<CollectionSections>();
                    ViewModel.Suggestions.Subscribe(observer);

                    var workspace = new MockWorkspace { Id = 1, Name = "ws", Admin = true };
                    InteractorFactory.GetAllWorkspaces().Execute().Returns(Observable.Return(new[] { workspace }));

                    await ViewModel.Initialize(DefaultParameter);
                    TestScheduler.Start();

                    var projectName = "bongo";
                    ViewModel.OnTextFieldInfoFromView(new QueryTextSpan($"@{projectName}", 6));

                    TestScheduler.Start();
                    observer.LastEmittedValue()
                        .Where(section => firstItemIsCreateSuggestion(section, projectName))
                        .Should().NotBeEmpty();
                }

                [Fact, LogIfTooSlow]
                public async Task ExistEvenIfAProjectWithSameNameExist()
                {
                    var observer = TestScheduler.CreateObserver<CollectionSections>();
                    ViewModel.Suggestions.Subscribe(observer);

                    var workspace = new MockWorkspace { Id = 1, Name = "ws", Admin = true };
                    InteractorFactory.GetAllWorkspaces().Execute().Returns(Observable.Return(new[] { workspace }));

                    await ViewModel.Initialize(DefaultParameter);
                    TestScheduler.Start();

                    ViewModel.OnTextFieldInfoFromView(new QueryTextSpan($"@{ProjectName}", 6));

                    TestScheduler.Start();
                    observer.LastEmittedValue()
                        .Where(section => firstItemIsCreateSuggestion(section, ProjectName))
                        .Should().NotBeEmpty();
                }

                [Fact, LogIfTooSlow]
                public async Task TracksProjectSelection()
                {
                    await ViewModel.Initialize(DefaultParameter);

                    ViewModel.OnTextFieldInfoFromView(new QueryTextSpan("abcde @fgh", 10));

                    TestScheduler.Start();
                    AnalyticsService.StartEntrySelectProject.Received().Track(ProjectTagSuggestionSource.TextField);
                }
            }

            public sealed class WhenSuggestingTags : CreateSuggestionCellModels
            {
                protected override int MaxLength => MaxTagNameLengthInBytes;
                protected override char QuerySymbol => '#';
                protected override string QueryWithExactSuggestionMatch => TagName;

                public WhenSuggestingTags()
                {
                    var tag = Substitute.For<IThreadSafeTag>();
                    tag.Id.Returns(20);
                    tag.Name.Returns(TagName);
                    var tagSuggestion = new TagSuggestion(tag);

                    InteractorFactory.GetAutocompleteSuggestions(
                        Arg.Is<QueryInfo>(info => info.SuggestionType == AutocompleteSuggestionType.Tags),
                        Arg.Any<DefaultTimeEntrySearchEngine>())
                        .Execute()
                        .Returns(Observable.Return(new TagSuggestion[] { tagSuggestion }));

                    ViewModel.Initialize(DefaultParameter);
                    ViewModel.Suggestions.Subscribe(); // Otherwise the observable chaing won't trigger

                }

                [Fact, LogIfTooSlow]
                public async Task ExistNoMatterThatAProjectIsAlreadySelected()
                {
                    var observer = TestScheduler.CreateObserver<CollectionSections>();
                    ViewModel.Suggestions.Subscribe(observer);
                    var projectSpan = new ProjectSpan(ProjectId, ProjectName, ProjectColor, null, null);
                    var tagName = "fgh";
                    var querySpan = new QueryTextSpan($"abcde #{tagName}", 10);

                    await ViewModel.Initialize(DefaultParameter);

                    ViewModel.SetTextSpans(ImmutableList.Create<ISpan>(projectSpan));
                    ViewModel.SetTextSpans(ImmutableList.Create<ISpan>(projectSpan, querySpan));

                    TestScheduler.Start();
                    observer.LastEmittedValue()
                        .Where(section => firstItemIsCreateSuggestion(section, tagName))
                        .Should().NotBeEmpty();
                }

                [Fact, LogIfTooSlow]
                public async Task ExistNoMatterThatAProjectIsAlreadySelectedAndInTagSuccestionMode()
                {
                    var projectSpan = new ProjectSpan(ProjectId, ProjectName, ProjectColor, null, null);
                    var tagName = "fgh";
                    var querySpan = new QueryTextSpan($"abcde #{tagName}", 10);

                    await ViewModel.Initialize(DefaultParameter);
                    var observer = TestScheduler.CreateObserver<CollectionSections>();
                    ViewModel.Suggestions.Subscribe(observer);

                    ViewModel.SetTextSpans(ImmutableList.Create<ISpan>(projectSpan));
                    ViewModel.ToggleTagSuggestions.ExecuteWithCompletion();
                    ViewModel.SetTextSpans(ImmutableList.Create<ISpan>(projectSpan, querySpan));

                    TestScheduler.Start();
                    observer.LastEmittedValue()
                        .Where(section => firstItemIsCreateSuggestion(section, tagName))
                        .Should().NotBeEmpty();
                }

                [Fact, LogIfTooSlow]
                public async Task WontExistIfAnOtherTagIsAvailableWithSameNameButDifferentCase()
                {
                    var observer = TestScheduler.CreateObserver<CollectionSections>();
                    ViewModel.Suggestions.Subscribe(observer);

                    var projectSpan = new ProjectSpan(ProjectId, ProjectName, ProjectColor, null, null);
                    var querySpan = new QueryTextSpan("#mobile", 7);

                    await ViewModel.Initialize(DefaultParameter);
                    ViewModel.OnTextFieldInfoFromView(projectSpan);
                    ViewModel.ToggleTagSuggestions.Execute();

                    ViewModel.OnTextFieldInfoFromView(projectSpan, querySpan);
                    TestScheduler.Start();

                    TestScheduler.Start();
                    observer.LastEmittedValue()
                        .Where(firstItemIsCreateSuggestion)
                        .Should().BeEmpty();
                }

                [Fact, LogIfTooSlow]
                public async Task TracksTagSelection()
                {
                    ViewModel.Initialize(DefaultParameter);
                    TestScheduler.Start();

                    ViewModel.OnTextFieldInfoFromView(new QueryTextSpan("abcde #fgh", 10));

                    AnalyticsService.StartEntrySelectTag.Received().Track(ProjectTagSuggestionSource.TextField);
                }
            }
        }

        public sealed class TheCreateProjectSuggestion
        {
            public sealed class WhenSuggestingProjects : StartTimeEntryViewModelTest
            {
                private const string currentQuery = "My awesome Toggl project";

                public WhenSuggestingProjects()
                {
                    ViewModel.Initialize(DefaultParameter).Wait();

                    var project = Substitute.For<IThreadSafeProject>();
                    project.Id.Returns(10);
                    InteractorFactory.GetProjectById(Arg.Any<long>()).Execute().Returns(Observable.Return(project));
                    ViewModel.OnTextFieldInfoFromView(
                        new QueryTextSpan($"@{currentQuery}", 15)
                    );

                    TestScheduler.Start();
                }

                [Fact, LogIfTooSlow]
                public async Task CallsTheCreateProjectViewModel()
                {
                    ViewModel.Suggestions.Subscribe();

                    ViewModel.SelectSuggestion.Execute(new CreateEntitySuggestion(Resources.CreateProject, ""));

                    TestScheduler.Start();
                    await NavigationService.Received()
                        .Navigate<EditProjectViewModel, string, long?>(Arg.Any<string>(), ViewModel.View);
                }

                [Fact, LogIfTooSlow]
                public async Task UsesTheCurrentQueryAsTheParameterForTheCreateProjectViewModel()
                {
                    ViewModel.Suggestions.Subscribe();

                    ViewModel.SelectSuggestion.Execute(new CreateEntitySuggestion(Resources.CreateProject, ""));

                    TestScheduler.Start();
                    await NavigationService.Received()
                        .Navigate<EditProjectViewModel, string, long?>(currentQuery, ViewModel.View);
                }

                [Fact, LogIfTooSlow]
                public async Task SelectsTheCreatedProject()
                {
                    long projectId = 200;
                    NavigationService
                        .Navigate<EditProjectViewModel, string, long?>(Arg.Is(currentQuery), ViewModel.View)
                        .Returns(projectId);
                    var project = Substitute.For<IThreadSafeProject>();
                    project.Id.Returns(projectId);
                    project.Name.Returns(currentQuery);
                    InteractorFactory.GetProjectById(Arg.Is(projectId)).Execute().Returns(Observable.Return(project));
                    ViewModel.Suggestions.Subscribe();

                    ViewModel.SelectSuggestion.Execute(new CreateEntitySuggestion(Resources.CreateProject, ""));

                    TestScheduler.Start();
                    var projectSpan = TextFieldInfoObserver.LastEmittedValue().GetProjectSpan();
                    projectSpan.ProjectName.Should().Be(currentQuery);
                }
            }
        }

        public sealed class WhenSuggestingTags : StartTimeEntryViewModelTest
        {
            private const string currentQuery = "My awesome Toggl project";

            private readonly QueryTextSpan querySpan = new QueryTextSpan($"#{currentQuery}", 1);

            public WhenSuggestingTags()
            {
                ViewModel.Initialize(DefaultParameter).Wait();
                TestScheduler.Start();
                ViewModel.OnTextFieldInfoFromView(querySpan);
                TestScheduler.Start();
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesTagWithCurrentQueryAsName()
            {
                ViewModel.Suggestions.Subscribe();

                ViewModel.SelectSuggestion.Execute(new CreateEntitySuggestion(Resources.CreateTag, ""));

                TestScheduler.Start();
                await InteractorFactory
                    .Received()
                    .CreateTag(Arg.Is(currentQuery), Arg.Any<long>())
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesTagInProjectsWorkspaceIfAProjectIsSelected()
            {
                ViewModel.Suggestions.Subscribe();

                long workspaceId = 100;
                long projectId = 101;
                var project = new MockProject
                {
                    Id = projectId,
                    WorkspaceId = workspaceId
                };
                var projectSpan = new ProjectSpan(projectId, "Project", "0000AF", null, null);
                var tagSuggestion = new CreateEntitySuggestion(Resources.CreateTag, "");

                InteractorFactory.GetProjectById(Arg.Is(projectId)).Execute()
                    .Returns(Observable.Return(project));

                ViewModel.SelectSuggestion.ExecuteWithCompletion(new ProjectSuggestion(project))
                    .Do(_ => ViewModel.SetTextSpans(ImmutableList.Create<ISpan>(querySpan, projectSpan)))
                    .PrependAction(ViewModel.SelectSuggestion, tagSuggestion)
                    .Subscribe();

                TestScheduler.Start();
                InteractorFactory
                    .Received()
                    .CreateTag(Arg.Any<string>(), Arg.Is(workspaceId))
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesTagInUsersDefaultWorkspaceIfNoProjectIsSelected()
            {
                long workspaceId = 100;
                var workspace = new MockWorkspace { Id = workspaceId };
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(workspace));
                var user = Substitute.For<IThreadSafeUser>();
                user.DefaultWorkspaceId.Returns(workspaceId);
                DataSource.User.Get().Returns(Observable.Return(user));
                await ViewModel.Initialize(DefaultParameter);

                ViewModel.SelectSuggestion.Execute(new CreateEntitySuggestion(Resources.CreateTag, ""));

                TestScheduler.Start();
                await InteractorFactory
                    .Received()
                    .CreateTag(Arg.Any<string>(), Arg.Is(workspaceId))
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task SelectsTheCreatedTag()
            {
                var tagId = 0;
                var tag = Substitute.For<IThreadSafeTag>();
                tag.Id.Returns(tagId);
                tag.Name.Returns(currentQuery);

                InteractorFactory
                    .CreateTag(Arg.Any<string>(), Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(tag));

                await ViewModel.Initialize(DefaultParameter);
                ViewModel.Suggestions.Subscribe();

                ViewModel.SelectSuggestion.Execute(new CreateEntitySuggestion(Resources.CreateTag, ""));

                TestScheduler.Start();
                var tags = TextFieldInfoObserver.LastEmittedValue().GetTagSpan();
                tags.TagName.Should().Be(currentQuery);
            }
        }

        public sealed class TheCloseWithDefaultResultMethod : StartTimeEntryViewModelTest
        {
            public TheCloseWithDefaultResultMethod()
            {
                var parameter = StartTimeEntryParameters.ForTimerMode(DateTimeOffset.Now, false);
                ViewModel.Initialize(parameter);
            }

            [Fact, LogIfTooSlow]
            public void ClosesTheViewModelIfUserDoesNotChangeAnything()
            {
                ViewModel.CloseWithDefaultResult();

                TestScheduler.Start();
                View.Received().Close();
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsAConfirmationDialogIfUserEnteredSomething()
            {
                makeDirty();

                ViewModel.CloseWithDefaultResult();

                TestScheduler.Start();
                await View.Received().ConfirmDestructiveAction(ActionType.DiscardNewTimeEntry);
            }

            [Fact, LogIfTooSlow]
            public void DoesNotCloseTheViewIfUserWantsToContinueEditing()
            {
                makeDirty();
                View.ConfirmDestructiveAction(ActionType.DiscardNewTimeEntry)
                    .Returns(_ => Observable.Return(false));

                ViewModel.CloseWithDefaultResult();

                TestScheduler.Start();
                View.DidNotReceive().Close();
            }

            [Fact, LogIfTooSlow]
            public void ClosesTheViewIfUserWantsToDiscardTheEnteredInformation()
            {
                makeDirty();
                View.ConfirmDestructiveAction(ActionType.DiscardNewTimeEntry)
                    .Returns(_ => Observable.Return(true));

                ViewModel.CloseWithDefaultResult();

                TestScheduler.Start();
                View.Received().Close();
            }

            [Fact, LogIfTooSlow]
            public void DoesNotCallTheAnalyticsServiceSinceNoTimeEntryWasCreated()
            {
                ViewModel.CloseWithDefaultResult();

                TestScheduler.Start();
                AnalyticsService.DidNotReceive().Track(Arg.Any<StartTimeEntryEvent>());
            }

            private void makeDirty()
            {
                ViewModel.ToggleBillable.Execute();
            }
        }

        public sealed class TheToggleBillableAction : StartTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void TogglesTheIsBillableProperty()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsBillable.Subscribe(observer);

                ViewModel.ToggleBillable.Execute();
                TestScheduler.Start();

                observer.Values().Count().Should().Be(2);
                var billableValues = observer.Values().ToList();
                billableValues[0].Should().Be(!billableValues[1]);
            }

            [Fact, LogIfTooSlow]
            public void TracksBillableTap()
            {
                ViewModel.ToggleBillable.Execute();

                AnalyticsService.Received()
                                .StartViewTapped
                                .Track(StartViewTapSource.Billable);
            }
        }

        public sealed class TheToggleProjectSuggestionsAction : StartTimeEntryViewModelTest
        {
            public TheToggleProjectSuggestionsAction()
            {
                var suggestions = ProjectSuggestion.FromProjects(Enumerable.Empty<IThreadSafeProject>());

                InteractorFactory.GetAutocompleteSuggestions(
                    Arg.Is<QueryInfo>(info => info.Text.Contains("@")),
                    Arg.Any<DefaultTimeEntrySearchEngine>())
                    .Execute()
                    .Returns(Observable.Return(suggestions));

                InteractorFactory.GetAutocompleteSuggestions(
                    Arg.Is<QueryInfo>(arg => arg.SuggestionType == AutocompleteSuggestionType.Projects),
                    Arg.Any<DefaultTimeEntrySearchEngine>())
                    .Execute()
                    .Returns(Observable.Return(suggestions));

                var defaultWorkspace = new MockWorkspace { Id = WorkspaceId };
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(defaultWorkspace));
            }

            private List<IThreadSafeProject> createProjects(int count)
                => Enumerable
                    .Range(0, count)
                    .Select(i =>
                    {
                        var workspace = Substitute.For<IThreadSafeWorkspace>();
                        workspace.Id.Returns(WorkspaceId);
                        workspace.Name.Returns(WorkspaceName);

                        var project = Substitute.For<IThreadSafeProject>();
                        project.Workspace.Returns(workspace);
                        project.WorkspaceId.Returns(WorkspaceId);
                        project.Id.Returns(ProjectId + i);
                        project.Name.Returns($"{ProjectName}-{i}");
                        project.Color.Returns(ProjectColor);
                        return project;
                    })
                    .ToList();

            [Fact, LogIfTooSlow]
            public async Task StartProjectSuggestionEvenIfTheProjectHasAlreadyBeenSelected()
            {
                ViewModel.Initialize(DefaultParameter);
                ViewModel.OnTextFieldInfoFromView(
                    new QueryTextSpan(Description, Description.Length),
                    new ProjectSpan(ProjectId, ProjectName, ProjectColor, null, null)
                );

                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsSuggestingProjects.Subscribe(observer);

                ViewModel.ToggleProjectSuggestions.Execute();

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void SetsTheIsSuggestingProjectsPropertyToTrueIfNotInProjectSuggestionMode()
            {
                var isSuggestingObserver = TestScheduler.CreateObserver<bool>();
                var suggestionObserver = TestScheduler.CreateObserver<IImmutableList<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Initialize(DefaultParameter);
                ViewModel.Suggestions.Subscribe(suggestionObserver);
                ViewModel.IsSuggestingProjects.Subscribe(isSuggestingObserver);

                ViewModel.ToggleProjectSuggestions.Execute();
                TestScheduler.Start();

                isSuggestingObserver.LastEmittedValue().Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsCorrectProjectSuggestionsAfterProjectHasAlreadyBeenSelected()
            {
                var projects = createProjects(5);
                var noProjectCount = 1;
                var projectSuggestions = ProjectSuggestion.FromProjects(projects);
                var chosenProject = projectSuggestions.First();
                DataSource.Projects.GetAll().Returns(Observable.Return(projects));
                InteractorFactory.GetAutocompleteSuggestions(
                    Arg.Any<QueryInfo>(),
                    Arg.Any<DefaultTimeEntrySearchEngine>())
                    .Execute()
                    .Returns(Observable.Return(projectSuggestions));
                await ViewModel.Initialize(DefaultParameter);
                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan(Description, Description.Length));
                var observer = TestScheduler
                    .CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                ViewModel.ToggleProjectSuggestions.ExecuteWithCompletion()
                    .PrependAction(ViewModel.SelectSuggestion, chosenProject)
                    .PrependAction(ViewModel.ToggleProjectSuggestions)
                    .Subscribe();

                TestScheduler.Start();
                observer.LastEmittedValue().Should().HaveCount(1);
                observer.LastEmittedValue().First().Items.Should().HaveCount(projects.Count + noProjectCount);
            }

            [Fact, LogIfTooSlow]
            public async Task ProjectSuggestionsAreClearedOnProjectSelection()
            {
                var projects = createProjects(5);
                var projectSuggestions = ProjectSuggestion.FromProjects(projects);
                var chosenProject = projectSuggestions.First();
                InteractorFactory.GetAutocompleteSuggestions(
                    Arg.Any<QueryInfo>(),
                    Arg.Any<DefaultTimeEntrySearchEngine>())
                    .Execute()
                    .Returns(Observable.Return(projectSuggestions));

                await ViewModel.Initialize(DefaultParameter);
                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan(Description, Description.Length));

                var suggestionsObserver = TestScheduler
                    .CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(suggestionsObserver);
                var isSuggestingProjectsObserver = TestScheduler.CreateObserver<bool>();
                ViewModel.IsSuggestingProjects.Subscribe(isSuggestingProjectsObserver);

                ViewModel.ToggleProjectSuggestions.Execute();
                ViewModel.SelectSuggestion.Execute(chosenProject);

                TestScheduler.Start();
                isSuggestingProjectsObserver.LastEmittedValue().Should().BeFalse();
                suggestionsObserver.LastEmittedValue().Should().HaveCount(0);
            }

            [Fact, LogIfTooSlow]
            public async Task AddsAnAtSymbolAtTheEndOfTheQueryInOrderToStartProjectSuggestionMode()
            {
                const string description = "Testing Toggl Apps";
                var expected = $"{description} @";
                ViewModel.Initialize(DefaultParameter);
                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan(description, description.Length));

                ViewModel.ToggleProjectSuggestions.Execute();

                TestScheduler.Start();
                var querySpan = TextFieldInfoObserver.LastEmittedValue().GetQuerySpan();
                querySpan.Text.Should().Be(expected);
            }

            [Theory, LogIfTooSlow]
            [InlineData("@")]
            [InlineData("@somequery")]
            [InlineData("@some query")]
            [InlineData("@some query@query")]
            [InlineData("Testing Toggl Apps @")]
            [InlineData("Testing Toggl Apps @somequery")]
            [InlineData("Testing Toggl Apps @some query")]
            [InlineData("Testing Toggl Apps @some query @query")]
            public void SetsTheIsSuggestingProjectsPropertyToFalseIfAlreadyInProjectSuggestionMode(string description)
            {
                ViewModel.Initialize(DefaultParameter);
                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan(description, description.Length));
                var isSuggestingProjectsObserver = TestScheduler.CreateObserver<bool>();
                ViewModel.IsSuggestingProjects.Subscribe(isSuggestingProjectsObserver);

                ViewModel.ToggleProjectSuggestions.Execute();

                TestScheduler.Start();
                isSuggestingProjectsObserver.LastEmittedValue().Should().BeFalse();
            }

            [Theory, LogIfTooSlow]
            [InlineData("@", "")]
            [InlineData("@somequery", "")]
            [InlineData("@some query", "")]
            [InlineData("@some query@query", "")]
            [InlineData("Testing Toggl Apps @", "Testing Toggl Apps ")]
            [InlineData("Testing Toggl Apps @somequery", "Testing Toggl Apps ")]
            [InlineData("Testing Toggl Apps @some query", "Testing Toggl Apps ")]
            [InlineData("Testing Toggl Apps @some query @query", "Testing Toggl Apps ")]
            public void RemovesTheAtSymbolFromTheDescriptionTextIfAlreadyInProjectSuggestionMode(
                string description, string expected)
            {
                ViewModel.Suggestions.Subscribe();

                ViewModel.Initialize(DefaultParameter);
                ViewModel.SetTextSpans(ImmutableList.Create<ISpan>(new QueryTextSpan(description, description.Length)));

                ViewModel.ToggleProjectSuggestions.Execute();

                TestScheduler.Start();
                var querySpan = TextFieldInfoObserver.LastEmittedValue().GetQuerySpan();
                querySpan.Text.Should().Be(expected);
            }

            [Fact, LogIfTooSlow]
            public void SetsProjectOrTagWasAdded()
            {
                ViewModel.ToggleProjectSuggestions.Execute();

                TestScheduler.Start();
                OnboardingStorage.Received().ProjectOrTagWasAdded();
            }

            [Fact, LogIfTooSlow]
            public void TracksShowProjectSuggestions()
            {
                ViewModel.ToggleProjectSuggestions.Execute();

                TestScheduler.Start();
                AnalyticsService.StartViewTapped.Received().Track(StartViewTapSource.Project);
                AnalyticsService.StartEntrySelectProject.Received().Track(ProjectTagSuggestionSource.ButtonOverKeyboard);
            }
        }

        public sealed class TheToggleTagSuggestionsCommand : StartTimeEntryViewModelTest
        {
            public TheToggleTagSuggestionsCommand()
            {
                var tag = Substitute.For<IThreadSafeTag>();
                tag.Id.Returns(TagId);
                tag.Name.Returns(TagName);
                var suggestions = TagSuggestion.FromTags(new[] { tag });

                InteractorFactory.GetAutocompleteSuggestions(
                    Arg.Is<QueryInfo>(info => info.Text.Contains("#")),
                    Arg.Any<DefaultTimeEntrySearchEngine>())
                    .Execute()
                    .Returns(Observable.Return(suggestions));
            }

            [Fact, LogIfTooSlow]
            public void SetsTheIsSuggestingTagsPropertyToTrueIfNotInTagSuggestionMode()
            {
                ViewModel.Initialize(DefaultParameter);
                ViewModel.Suggestions.Subscribe();
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsSuggestingTags.Subscribe(observer);

                ViewModel.ToggleTagSuggestions.Execute();

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task AddsHashtagSymbolAtTheEndOfTheQueryInOrderToTagSuggestionMode()
            {
                const string description = "Testing Toggl Apps";
                var expected = $"{description} #";
                ViewModel.Initialize(DefaultParameter);
                ViewModel.Suggestions.Subscribe();
                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan(description, description.Length));

                ViewModel.ToggleTagSuggestions.Execute();

                TestScheduler.Start();
                var querySpan = TextFieldInfoObserver.LastEmittedValue().GetQuerySpan();
                querySpan.Text.Should().Be(expected);
            }

            [Theory, LogIfTooSlow]
            [InlineData("#")]
            [InlineData("#somequery")]
            [InlineData("#some query")]
            [InlineData("#some quer#query")]
            [InlineData("Testing Toggl Apps #")]
            [InlineData("Testing Toggl Apps #somequery")]
            [InlineData("Testing Toggl Apps #some query")]
            [InlineData("Testing Toggl Apps #some query #query")]
            public async Task SetsTheIsSuggestingTagsPropertyToFalseIfAlreadyInTagSuggestionMode(string description)
            {
                ViewModel.Initialize(DefaultParameter);
                ViewModel.Suggestions.Subscribe();
                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan(description, description.Length));
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsSuggestingTags.Subscribe(observer);

                ViewModel.ToggleTagSuggestions.Execute();

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeFalse();
            }

            [Theory, LogIfTooSlow]
            [InlineData("#", "")]
            [InlineData("#somequery", "")]
            [InlineData("#some query", "")]
            [InlineData("#some query#query", "")]
            [InlineData("Testing Toggl Apps #", "Testing Toggl Apps ")]
            [InlineData("Testing Toggl Apps #somequery", "Testing Toggl Apps ")]
            [InlineData("Testing Toggl Apps #some query", "Testing Toggl Apps ")]
            [InlineData("Testing Toggl Apps #some query #query", "Testing Toggl Apps ")]
            public async Task RemovesTheHashtagSymbolFromTheDescriptionTextIfAlreadyInTagSuggestionMode(
                string description, string expected)
            {
                ViewModel.Suggestions.Subscribe();

                var observer = TestScheduler.CreateObserver<TextFieldInfo>();
                ViewModel.TextFieldInfo.Subscribe(observer);

                ViewModel.Initialize(DefaultParameter);
                ViewModel.Suggestions.Subscribe();

                ViewModel.SetTextSpans(ImmutableList.Create<ISpan>(new ProjectSpan(ProjectId, ProjectName, ProjectColor)));
                ViewModel.SetTextSpans(ImmutableList.Create<ISpan>(new QueryTextSpan(description, description.Length)));
                ViewModel.ToggleTagSuggestions.ExecuteWithCompletion();

                TestScheduler.Start();

                var querySpan = observer.LastEmittedValue().GetQuerySpan();
                querySpan.Text.Should().Be(expected);
            }

            [Fact, LogIfTooSlow]
            public void SetsProjectOrTagWasAdded()
            {
                ViewModel.ToggleTagSuggestions.Execute();

                TestScheduler.Start();
                OnboardingStorage.Received().ProjectOrTagWasAdded();
            }

            [Fact, LogIfTooSlow]
            public void TracksShowTagSuggestions()
            {
                ViewModel.ToggleTagSuggestions.Execute();

                TestScheduler.Start();
                AnalyticsService.StartViewTapped.Received().Track(StartViewTapSource.Tags);
                AnalyticsService.StartEntrySelectTag.Received().Track(ProjectTagSuggestionSource.ButtonOverKeyboard);
            }


        }

        public sealed class TheChangeTimeCommand : StartTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task TracksStartTimeTap()
            {
                var now = DateTimeOffset.UtcNow;
                var parameters = StartTimeEntryParameters.ForTimerMode(now, false);
                var returnParameter = DurationParameter.WithStartAndDuration(now, TimeSpan.FromMinutes(1));
                NavigationService
                    .Navigate<EditDurationViewModel, EditDurationParameters, DurationParameter>(Arg.Any<EditDurationParameters>(), ViewModel.View)
                    .Returns(Task.FromResult(returnParameter));
                ViewModel.Initialize(parameters);

                ViewModel.ChangeTime.Execute();

                TestScheduler.Start();
                AnalyticsService.StartViewTapped.Received().Track(StartViewTapSource.StartTime);
            }
        }

        public sealed class TheSetStartDateCommand : StartTimeEntryViewModelTest
        {
            private static readonly DateTimeOffset now = new DateTimeOffset(2018, 02, 13, 23, 59, 12, TimeSpan.FromHours(-1));
            private static readonly StartTimeEntryParameters prepareParameters = StartTimeEntryParameters.ForTimerMode(now, false);

            public TheSetStartDateCommand()
            {
                TimeService.CurrentDateTime.Returns(now);
            }

            [Fact, LogIfTooSlow]
            public async Task NavigatesToTheSelectDateTimeViewModel()
            {
                ViewModel.Initialize(prepareParameters);

                ViewModel.SetStartDate.Execute();

                TestScheduler.Start();
                await NavigationService.Received().Navigate<SelectDateTimeViewModel, DateTimePickerParameters, DateTimeOffset>(Arg.Any<DateTimePickerParameters>(), ViewModel.View);
            }

            [Fact]
            public async Task OpensTheSelectDateTimeViewModelWithCorrectLimitsForARunningTimeEntry()
            {
                ViewModel.Initialize(prepareParameters);

                ViewModel.SetStartDate.Execute();

                TestScheduler.Start();
                await NavigationService.Received()
                    .Navigate<SelectDateTimeViewModel, DateTimePickerParameters, DateTimeOffset>(
                        Arg.Is<DateTimePickerParameters>(param => param.MinDate == now - MaxTimeEntryDuration && param.MaxDate == now), ViewModel.View);
            }

            [Fact]
            public async Task OpensTheSelectDateTimeViewModelWithCorrectLimitsForAStoppedTimeEntry()
            {
                var stoppedParametsrs = StartTimeEntryParameters.ForManualMode(now, false);
                ViewModel.Initialize(stoppedParametsrs);

                ViewModel.SetStartDate.Execute();

                TestScheduler.Start();
                await NavigationService.Received()
                    .Navigate<SelectDateTimeViewModel, DateTimePickerParameters, DateTimeOffset>(
                        Arg.Is<DateTimePickerParameters>(param => param.MinDate == EarliestAllowedStartTime && param.MaxDate == LatestAllowedStartTime), ViewModel.View);
            }

            [Fact, LogIfTooSlow]
            public async Task TracksStartDateTap()
            {
                TimeService.CurrentDateTime.Returns(now);
                NavigationService
                    .Navigate<SelectDateTimeViewModel, DateTimePickerParameters, DateTimeOffset>(Arg.Any<DateTimePickerParameters>(), ViewModel.View)
                    .Returns(now);
                ViewModel.Initialize(prepareParameters);

                ViewModel.SetStartDate.Execute();

                TestScheduler.Start();
                AnalyticsService.StartViewTapped.Received().Track(StartViewTapSource.StartDate);
            }
        }

        public sealed class TheDoneCommand : StartTimeEntryViewModelTest
        {
            private const long userId = 10;
            private const long projectId = 11;
            private const long defaultWorkspaceId = 12;
            private const long projectWorkspaceId = 13;

            private readonly IThreadSafeUser user;
            private readonly IThreadSafeProject project;

            private readonly DateTimeOffset startDate = DateTimeOffset.UtcNow;

            public TheDoneCommand()
            {
                var defaultWorkspace = new MockWorkspace { Id = defaultWorkspaceId };
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(defaultWorkspace));

                user = new MockUser
                {
                    Id = userId,
                    DefaultWorkspaceId = defaultWorkspaceId
                };

                project = new MockProject
                {
                    Id = projectId,
                    WorkspaceId = projectWorkspaceId
                };

                DataSource.User.Current
                    .Returns(Observable.Return(user));

                InteractorFactory
                    .GetProjectById(projectId)
                    .Execute()
                    .Returns(Observable.Return(project));

                var parameter = new StartTimeEntryParameters(startDate, "", null, null);
                ViewModel.Initialize(parameter).GetAwaiter().GetResult();
            }

            [Fact, LogIfTooSlow]
            public async Task CallsTheCreateTimeEntryInteractor()
            {
                ViewModel.Done.Execute(Unit.Default);

                TestScheduler.Start();
                var te =
                    InteractorFactory.Received().CreateTimeEntry(Arg.Any<ITimeEntryPrototype>(), TimeEntryStartOrigin.Timer);
            }

            [Fact, LogIfTooSlow]
            public async Task ExecutesTheCreateTimeEntryInteractor()
            {
                var mockedInteractor = Substitute.For<IInteractor<Task<IThreadSafeTimeEntry>>>();
                InteractorFactory.CreateTimeEntry(Arg.Any<ITimeEntryPrototype>(), TimeEntryStartOrigin.Timer).Returns(mockedInteractor);

                ViewModel.Done.Execute(Unit.Default);

                TestScheduler.Start();
                await mockedInteractor.Received().Execute();
            }

            [Theory, LogIfTooSlow]
            [InlineData(" ")]
            [InlineData("\t")]
            [InlineData("\n")]
            [InlineData("               ")]
            [InlineData("      \t  \n     ")]
            public void ReducesDescriptionConsistingOfOnlyEmptyCharactersToAnEmptyString(string description)
            {
                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan(description, 0));

                ViewModel.Done.Execute(Unit.Default);

                TestScheduler.Start();
                InteractorFactory.Received().CreateTimeEntry(Arg.Is<ITimeEntryPrototype>(timeEntry =>
                    timeEntry.Description.Length == 0
                ), TimeEntryStartOrigin.Timer);
            }

            [Theory, LogIfTooSlow]
            [InlineData("   abcde", "abcde")]
            [InlineData("abcde     ", "abcde")]
            [InlineData("  abcde ", "abcde")]
            [InlineData("abcde  fgh", "abcde  fgh")]
            [InlineData("      abcd\nefgh     ", "abcd\nefgh")]
            public void TrimsDescriptionFromTheStartAndTheEndBeforeSaving(string description, string trimmed)
            {
                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan(description, description.Length));

                ViewModel.Done.Execute(Unit.Default);
                TestScheduler.Start();

                InteractorFactory.Received().CreateTimeEntry(Arg.Is<ITimeEntryPrototype>(timeEntry =>
                    timeEntry.Description == trimmed
                ), TimeEntryStartOrigin.Timer);
            }

            [Property]
            public void DoesNotCreateARunningTimeEntryWhenDurationIsNotNull(TimeSpan duration)
            {
                if (duration < TimeSpan.Zero) return;

                var parameter = new StartTimeEntryParameters(DateTimeOffset.Now, "", duration, null);

                ViewModel.Initialize(parameter);
                ViewModel.Done.Execute(Unit.Default);

                TestScheduler.Start();

                InteractorFactory.Received().CreateTimeEntry(Arg.Is<ITimeEntryPrototype>(timeEntry =>
                    timeEntry.Duration.HasValue
                ), TimeEntryStartOrigin.Manual);
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesARunningTimeEntryWhenDurationIsNull()
            {
                var parameter = new StartTimeEntryParameters(DateTimeOffset.Now, "", null, null);

                ViewModel.Initialize(parameter);
                ViewModel.Done.Execute(Unit.Default);

                TestScheduler.Start();
                InteractorFactory.Received().CreateTimeEntry(Arg.Is<ITimeEntryPrototype>(timeEntry =>
                    timeEntry.Duration.HasValue == false
                ), TimeEntryStartOrigin.Timer);
            }

            private TagSuggestion tagSuggestionFromInt(int i)
            {
                var tag = Substitute.For<IThreadSafeTag>();
                tag.Id.Returns(i);
                tag.Name.Returns(i.ToString());

                return new TagSuggestion(tag);
            }

            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModel()
            {
                var user = Substitute.For<IThreadSafeUser>();
                user.Id.Returns(1);
                user.DefaultWorkspaceId.Returns(10);
                DataSource.User.Current.Returns(Observable.Return(user));
                var parameter = new StartTimeEntryParameters(DateTimeOffset.Now, "", null, null);
                await ViewModel.Initialize(parameter);

                ViewModel.Done.Execute(Unit.Default);

                TestScheduler.Start();
                View.Received().Close();
            }
        }

        public sealed class TheSelectSuggestionCommand
        {
            public abstract class SelectSuggestionTest<TSuggestion> : StartTimeEntryViewModelTest
                where TSuggestion : AutocompleteSuggestion
            {
                protected IThreadSafeTag Tag { get; }
                protected IThreadSafeTask Task { get; }
                protected IThreadSafeProject Project { get; }
                protected IThreadSafeTimeEntry TimeEntry { get; }
                protected IThreadSafeWorkspace Workspace { get; }

                protected abstract TSuggestion Suggestion { get; }

                protected SelectSuggestionTest()
                {
                    Workspace = Substitute.For<IThreadSafeWorkspace>();
                    Workspace.Id.Returns(WorkspaceId);

                    Project = Substitute.For<IThreadSafeProject>();
                    Project.Id.Returns(ProjectId);
                    Project.Name.Returns(ProjectName);
                    Project.Color.Returns(ProjectColor);
                    Project.Workspace.Returns(Workspace);
                    Project.WorkspaceId.Returns(WorkspaceId);
                    Project.Active.Returns(true);

                    Task = Substitute.For<IThreadSafeTask>();
                    Task.Id.Returns(TaskId);
                    Task.Project.Returns(Project);
                    Task.ProjectId.Returns(ProjectId);
                    Task.WorkspaceId.Returns(WorkspaceId);
                    Task.Name.Returns(TaskId.ToString());

                    TimeEntry = Substitute.For<IThreadSafeTimeEntry>();
                    TimeEntry.Description.Returns(Description);
                    TimeEntry.Project.Returns(Project);

                    Tag = Substitute.For<IThreadSafeTag>();
                    Tag.Id.Returns(TagId);
                    Tag.Name.Returns(TagName);
                }

                [Fact, LogIfTooSlow]
                public async Task TracksProjectSelectionWhenProjectSymbolSelected()
                {
                    var projectSuggestion = QuerySymbolSuggestion.Suggestions
                        .First((s) => s.Symbol == QuerySymbols.ProjectsString);

                    ViewModel.SelectSuggestion.Execute(projectSuggestion);

                    TestScheduler.Start();
                    AnalyticsService.StartViewTapped.Received()
                        .Track(StartViewTapSource.PickEmptyStateProjectSuggestion);
                    AnalyticsService.StartEntrySelectProject.Received()
                        .Track(ProjectTagSuggestionSource.TableCellButton);
                }

                [Fact, LogIfTooSlow]
                public async Task TracksTagSelectionWhenTagSymbolSelected()
                {
                    var tagSuggestion = QuerySymbolSuggestion.Suggestions
                        .First((s) => s.Symbol == QuerySymbols.TagsString);

                    ViewModel.SelectSuggestion.Execute(tagSuggestion);

                    TestScheduler.Start();
                    AnalyticsService.StartViewTapped.Received().Track(StartViewTapSource.PickEmptyStateTagSuggestion);
                    AnalyticsService.StartEntrySelectTag.Received().Track(ProjectTagSuggestionSource.TableCellButton);
                }
            }

            public abstract class ProjectSettingSuggestion<TSuggestion> : SelectSuggestionTest<TSuggestion>
                where TSuggestion : AutocompleteSuggestion
            {
                [Fact, LogIfTooSlow]
                public void SetsTheProjectIdToTheSuggestedProjectId()
                {
                    ViewModel.SelectSuggestion.Execute(Suggestion);

                    TestScheduler.Start();
                    var projectSpan = TextFieldInfoObserver.LastEmittedValue().GetProjectSpan();
                    projectSpan.ProjectId.Should().Be(ProjectId);
                }

                [Fact, LogIfTooSlow]
                public void SetsTheProjectNameToTheSuggestedProjectName()
                {
                    ViewModel.SelectSuggestion.Execute(Suggestion);

                    TestScheduler.Start();
                    var projectSpan = TextFieldInfoObserver.LastEmittedValue().GetProjectSpan();
                    projectSpan.ProjectName.Should().Be(ProjectName);
                }

                [Fact, LogIfTooSlow]
                public void SetsTheProjectColorToTheSuggestedProjectColor()
                {
                    ViewModel.SelectSuggestion.Execute(Suggestion);
                    TestScheduler.Start();

                    var projectSpan = TextFieldInfoObserver.LastEmittedValue().GetProjectSpan();
                    projectSpan.ProjectColor.Should().Be(ProjectColor);
                }

                [Theory, LogIfTooSlow]
                [InlineData(true)]
                [InlineData(false)]
                public void SetsTheAppropriateBillableValue(bool billableValue)
                {
                    var isBillableObserver = TestScheduler.CreateObserver<bool>();
                    var isBillableAvailableObserver = TestScheduler.CreateObserver<bool>();
                    ViewModel.Suggestions.Subscribe();
                    ViewModel.IsBillable.Subscribe(isBillableObserver);
                    ViewModel.IsBillableAvailable.Subscribe(isBillableAvailableObserver);

                    InteractorFactory.GetWorkspaceById(WorkspaceId).Execute().Returns(Observable.Return(Workspace));
                    InteractorFactory.IsBillableAvailableForProject(ProjectId).Execute()
                        .Returns(Observable.Return(true));
                    InteractorFactory.ProjectDefaultsToBillable(ProjectId).Execute()
                        .Returns(Observable.Return(billableValue));

                    ViewModel.SelectSuggestion.Execute(Suggestion);
                    TestScheduler.Start();

                    isBillableObserver.LastEmittedValue().Should().Be(billableValue);
                    isBillableAvailableObserver.LastEmittedValue().Should().BeTrue();
                }

                [Theory, LogIfTooSlow]
                [InlineData(true)]
                [InlineData(false)]
                [InlineData(null)]
                public async Task DisablesBillableIfTheWorkspaceOfTheSelectedProjectDoesNotAllowIt(bool? billableValue)
                {
                    var isBillableObserver = TestScheduler.CreateObserver<bool>();
                    var isBillableAvailableObserver = TestScheduler.CreateObserver<bool>();
                    ViewModel.IsBillable.Subscribe(isBillableObserver);
                    ViewModel.IsBillableAvailable.Subscribe(isBillableAvailableObserver);

                    Project.Billable.Returns(billableValue);
                    InteractorFactory.GetProjectById(ProjectId).Execute().Returns(Observable.Return(Project));
                    InteractorFactory.GetWorkspaceById(WorkspaceId).Execute().Returns(Observable.Return(Workspace));
                    InteractorFactory
                        .IsBillableAvailableForWorkspace(10)
                        .Execute()
                        .Returns(Observable.Return(false));

                    ViewModel.SelectSuggestion.Execute(Suggestion);

                    TestScheduler.Start();
                    isBillableObserver.LastEmittedValue().Should().BeFalse();
                    isBillableAvailableObserver.LastEmittedValue().Should().BeFalse();
                }
            }

            public abstract class ProjectTaskSuggestion<TSuggestion> : ProjectSettingSuggestion<TSuggestion>
                where TSuggestion : AutocompleteSuggestion
            {
                protected ProjectTaskSuggestion()
                {
                    ViewModel.OnTextFieldInfoFromView(new QueryTextSpan("Something @togg", 15));
                }

                [Fact, LogIfTooSlow]
                public async Task RemovesTheProjectQueryFromTheTextFieldInfo()
                {
                    ViewModel.SelectSuggestion.Execute(Suggestion);

                    TestScheduler.Start();
                    var values = TextFieldInfoObserver.Values();
                    var querySpan = TextFieldInfoObserver.LastEmittedValue().FirstTextSpan();
                    querySpan.Text.Should().Be("Something ");
                }

                [Fact, LogIfTooSlow]
                public async Task ShowsConfirmDialogIfWorkspaceIsAboutToBeChanged()
                {
                    var user = Substitute.For<IThreadSafeUser>();
                    user.DefaultWorkspaceId.Returns(100);
                    DataSource.User.Current.Returns(Observable.Return(user));
                    await ViewModel.Initialize(DefaultParameter);

                    ViewModel.SelectSuggestion.Execute(Suggestion);

                    TestScheduler.Start();
                    await View.Received().Confirm(
                        Arg.Is(Resources.DifferentWorkspaceAlertTitle),
                        Arg.Is(Resources.DifferentWorkspaceAlertMessage),
                        Arg.Is(Resources.Ok),
                        Arg.Is(Resources.Cancel)
                    );
                }

                [Fact, LogIfTooSlow]
                public async Task DoesNotShowConfirmDialogIfWorkspaceIsNotGoingToChange()
                {
                    var workspace = new MockWorkspace { Id = WorkspaceId };
                    InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(workspace));
                    var user = Substitute.For<IThreadSafeUser>();
                    user.DefaultWorkspaceId.Returns(WorkspaceId);
                    DataSource.User.Current.Returns(Observable.Return(user));
                    await ViewModel.Initialize(DefaultParameter);

                    ViewModel.SelectSuggestion.Execute(Suggestion);

                    TestScheduler.Start();
                    await View.DidNotReceive().Confirm(
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<string>(),
                        Arg.Any<string>()
                    );
                }

                [Fact, LogIfTooSlow]
                public void ClearsTagsIfWorkspaceIsChanged()
                {
                    var user = Substitute.For<IThreadSafeUser>();
                    user.DefaultWorkspaceId.Returns(100);
                    DataSource.User.Current.Returns(Observable.Return(user));
                    ViewModel.Initialize(DefaultParameter);
                    var suggestions = Enumerable.Range(100, 10)
                        .Select(i =>
                        {
                            var tag = Substitute.For<IThreadSafeTag>();
                            tag.Id.Returns(i);
                            return new TagSuggestion(tag);
                        });

                    ViewModel.SelectSuggestion.ExecuteSequentally(suggestions)
                        .PrependAction(ViewModel.SelectSuggestion, Suggestion)
                        .Subscribe();

                    TestScheduler.Start();
                    var tags = TextFieldInfoObserver.LastEmittedValue().Spans.OfType<TagSpan>();
                    tags.Should().BeEmpty();
                }
            }

            public sealed class WhenSelectingATimeEntrySuggestion : ProjectSettingSuggestion<TimeEntrySuggestion>
            {
                protected override TimeEntrySuggestion Suggestion { get; }

                public WhenSelectingATimeEntrySuggestion()
                {
                    Suggestion = new TimeEntrySuggestion(TimeEntry);
                }

                [Fact, LogIfTooSlow]
                public async Task SetsTheTextFieldInfoTextToTheValueOfTheSuggestedDescription()
                {
                    ViewModel.SelectSuggestion.Execute(Suggestion);

                    TestScheduler.Start();
                    var querySpan = TextFieldInfoObserver.LastEmittedValue().FirstTextSpan();
                    querySpan.Text.Should().Be(Description);
                }

                [Fact, LogIfTooSlow]
                public async Task TracksWhenTimeEntrySuggestionSelected()
                {
                    ViewModel.SelectSuggestion.Execute(Suggestion);

                    TestScheduler.Start();
                    AnalyticsService.StartViewTapped.Received().Track(StartViewTapSource.PickTimeEntrySuggestion);
                }

                [Fact, LogIfTooSlow]
                public async Task ChangesTheWorkspaceIfNeeded()
                {
                    const long expectedWorkspaceId = WorkspaceId + 1;
                    TimeEntry.WorkspaceId.Returns(expectedWorkspaceId);
                    var newSuggestion = new TimeEntrySuggestion(TimeEntry);

                    ViewModel.SelectSuggestion.Execute(newSuggestion);

                    TestScheduler.Start();
                    TextFieldInfoObserver.LastEmittedValue().WorkspaceId.Should().Be(expectedWorkspaceId);
                }
            }

            public sealed class WhenSelectingATaskSuggestion : ProjectTaskSuggestion<TaskSuggestion>
            {
                protected override TaskSuggestion Suggestion { get; }

                public WhenSelectingATaskSuggestion()
                {
                    Suggestion = new TaskSuggestion(Task);
                }

                [Fact, LogIfTooSlow]
                public async Task SetsTheTaskIdToTheSameIdAsTheSelectedSuggestion()
                {
                    ViewModel.SelectSuggestion.Execute(Suggestion);

                    TestScheduler.Start();
                    var projectSpan = TextFieldInfoObserver.LastEmittedValue().GetProjectSpan();
                    projectSpan.TaskId.Should().Be(TaskId);
                }

                [Fact, LogIfTooSlow]
                public async Task TracksWhenTaskSuggestionSelected()
                {
                    ViewModel.SelectSuggestion.Execute(Suggestion);

                    TestScheduler.Start();
                    AnalyticsService.StartViewTapped.Received().Track(StartViewTapSource.PickTaskSuggestion);
                }
            }

            public sealed class WhenSelectingAProjectSuggestion : ProjectTaskSuggestion<ProjectSuggestion>
            {
                protected override ProjectSuggestion Suggestion { get; }

                public WhenSelectingAProjectSuggestion()
                {
                    Suggestion = new ProjectSuggestion(Project);
                }

                [Fact, LogIfTooSlow]
                public async Task SetsTheTaskIdToNull()
                {
                    ViewModel.SelectSuggestion.Execute(Suggestion);

                    TestScheduler.Start();
                    var projectSpan = TextFieldInfoObserver.LastEmittedValue().GetProjectSpan();
                    projectSpan.TaskId.Should().BeNull();
                }

                [Theory, LogIfTooSlow]
                [InlineData(true)]
                [InlineData(false)]
                public void SetsTheAppropriateBillableValueBasedOnTheWorkspaceWhenSelectingNoProject(bool isBillableAvailable)
                {
                    var noProjectSuggestion = ProjectSuggestion.NoProject(WorkspaceId, Workspace.Name);
                    var isBillableObserver = TestScheduler.CreateObserver<bool>();
                    var isBillableAvailableObserver = TestScheduler.CreateObserver<bool>();
                    ViewModel.Suggestions.Subscribe();
                    ViewModel.IsBillable.Subscribe(isBillableObserver);
                    ViewModel.IsBillableAvailable.Subscribe(isBillableAvailableObserver);
                    InteractorFactory
                        .IsBillableAvailableForWorkspace(WorkspaceId)
                        .Execute()
                        .Returns(Observable.Return(isBillableAvailable));

                    ViewModel.SelectSuggestion.Execute(noProjectSuggestion);
                    TestScheduler.Start();

                    isBillableObserver.LastEmittedValue().Should().BeFalse();
                    isBillableAvailableObserver.LastEmittedValue().Should().Be(isBillableAvailable);
                }

                [Fact, LogIfTooSlow]
                public async Task TracksWhenProjectSuggestionSelected()
                {
                    View
                        .Confirm(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                        .Returns(Observable.Return(false));

                    ViewModel.SelectSuggestion.Execute(Suggestion);

                    TestScheduler.Start();
                    AnalyticsService.StartViewTapped.Received().Track(StartViewTapSource.PickProjectSuggestion);
                }
            }

            public sealed class WhenSelectingATagSuggestion : SelectSuggestionTest<TagSuggestion>
            {
                protected override TagSuggestion Suggestion { get; }

                public WhenSelectingATagSuggestion()
                {
                    Suggestion = new TagSuggestion(Tag);

                    ViewModel.OnTextFieldInfoFromView(new QueryTextSpan("Something #togg", 15));
                }

                [Fact, LogIfTooSlow]
                public async Task RemovesTheTagQueryFromTheTextFieldInfo()
                {
                    ViewModel.SelectSuggestion.Execute(Suggestion);

                    TestScheduler.Start();
                    var values = TextFieldInfoObserver.Values().ToList();
                    var querySpan = TextFieldInfoObserver.LastEmittedValue().Spans.OfType<TextSpan>().First();
                    querySpan.Text.Should().Be("Something ");
                }

                [Fact, LogIfTooSlow]
                public async Task AddsTheSuggestedTagToTheList()
                {
                    ViewModel.SelectSuggestion.Execute(Suggestion);

                    TestScheduler.Start();
                    var tags = TextFieldInfoObserver.LastEmittedValue().Spans.OfType<TagSpan>();
                    tags.Should().Contain(t => t.TagId == Suggestion.TagId);
                }

                [Fact, LogIfTooSlow]
                public async Task TracksWhenTagSuggestionSelected()
                {
                    ViewModel.SelectSuggestion.Execute(Suggestion);

                    TestScheduler.Start();
                    AnalyticsService.StartViewTapped.Received().Track(StartViewTapSource.PickTagSuggestion);
                }
            }

            public sealed class WhenSelectingAQuerySymbolSuggestion : SelectSuggestionTest<QuerySymbolSuggestion>
            {
                protected override QuerySymbolSuggestion Suggestion { get; } = QuerySymbolSuggestion.Suggestions.First();

                [Fact, LogIfTooSlow]
                public async Task SetsTheTextToTheQuerySymbolSelected()
                {
                    ViewModel.SelectSuggestion.Execute(Suggestion);

                    TestScheduler.Start();
                    var querySpan = TextFieldInfoObserver.LastEmittedValue().FirstTextSpan();
                    querySpan.Text.Should().Be(Suggestion.Symbol);
                }
            }

            public sealed class WhenSelectingACreateEntitySuggestion : StartTimeEntryViewModelTest
            {
                [Fact, LogIfTooSlow]
                public void RemovesTheQuerySymbolWhenCreateingATag()
                {
                    var suggestionsObserver = TestScheduler.CreateObserver<IImmutableList<SectionModel<string, AutocompleteSuggestion>>>();
                    ViewModel.Suggestions.Subscribe(suggestionsObserver);
                    var textFieldInfoObserver = TestScheduler.CreateObserver<TextFieldInfo>();
                    ViewModel.TextFieldInfo.Subscribe(textFieldInfoObserver);
                    var spans = new ISpan[] {new QueryTextSpan("#NewTag", 7)}.ToImmutableList();
                    ViewModel.SetTextSpans(spans);
                    TestScheduler.Start();
                    var suggestionToTap = suggestionsObserver.LastEmittedValue().First().Items.Single(suggestion => suggestion is CreateEntitySuggestion);

                    ViewModel.SelectSuggestion.Execute(suggestionToTap);
                    TestScheduler.Start();

                    textFieldInfoObserver.LastEmittedValue().Description.Should().NotContain("#");
                }
            }
        }

        public sealed class TheDurationTappedCommand : StartTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void TracksDurationTap()
            {
                ViewModel.DurationTapped.Execute();

                TestScheduler.Start();
                AnalyticsService.StartViewTapped.Received().Track(StartViewTapSource.Duration);
            }
        }

        public sealed class TheSuggestionsProperty : StartTimeEntryViewModelTest
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
                var workspace = Substitute.For<IThreadSafeWorkspace>();
                workspace.Name.Returns($"Workspace{workspaceId}");
                workspace.Id.Returns(workspaceId);
                var project = Substitute.For<IThreadSafeProject>();
                project.WorkspaceId.Returns(workspaceId);
                project.Name.Returns($"Project{projectId}");
                project.Workspace.Returns(workspace);
                project.Active.Returns(true);
                project.Tasks.Returns(tasks);
                return new ProjectSuggestion(project);
            }

            private IEnumerable<string> projectNames(IEnumerable<AutocompleteSuggestion> autocompleteSuggestions)
                => autocompleteSuggestions.Cast<ProjectSuggestion>().Select(suggestion => suggestion.ProjectName);

            private IEnumerable<string> tasksNames(IEnumerable<AutocompleteSuggestion> autocompleteSuggestions)
                => autocompleteSuggestions.Cast<TaskSuggestion>().Select(suggestion => suggestion.Name);

            [Fact, LogIfTooSlow]
            public async Task PrependsEmptyProjectToEveryGroup()
            {
                var suggestions = new List<ProjectSuggestion>();
                suggestions.AddRange(getProjectSuggestions(3, 0));
                suggestions.AddRange(getProjectSuggestions(4, 1));
                suggestions.AddRange(getProjectSuggestions(1, 10));
                suggestions.AddRange(getProjectSuggestions(10, 54));
                var suggestionsObservable = Observable.Return(suggestions);
                InteractorFactory.GetAutocompleteSuggestions(
                    Arg.Any<QueryInfo>(),
                    Arg.Any<DefaultTimeEntrySearchEngine>())
                    .Execute()
                    .Returns(suggestionsObservable);

                var observer = TestScheduler
                    .CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);

                TestScheduler.Start();
                foreach (var group in observer.LastEmittedValue())
                {
                    group.Items.Cast<ProjectSuggestion>().First().ProjectName.Should().Be(Resources.NoProject);
                }
            }

            [Fact, LogIfTooSlow]
            public async Task IsClearedWhenThereAreNoWordsToQuery()
            {
                var observer = TestScheduler
                    .CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan("", 0));

                TestScheduler.Start();
                observer.LastEmittedValue().Should().HaveCount(0);
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotSuggestAnythingWhenAProjectIsAlreadySelected()
            {
                var description = "abc";
                var projectA = Substitute.For<IThreadSafeProject>();
                projectA.Id.Returns(ProjectId);
                var projectB = Substitute.For<IThreadSafeProject>();
                projectB.Id.Returns(ProjectId + 1);
                var timeEntryA = Substitute.For<IThreadSafeTimeEntry>();
                timeEntryA.Description.Returns(description);
                timeEntryA.Project.Returns(projectA);
                var timeEntryB = Substitute.For<IThreadSafeTimeEntry>();
                timeEntryB.Description.Returns(description);
                timeEntryB.Project.Returns(projectB);

                var observer = TestScheduler
                    .CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                var suggestions = Observable.Return(new AutocompleteSuggestion[]
                    {
                        new TimeEntrySuggestion(timeEntryA),
                        new TimeEntrySuggestion(timeEntryB)
                    });
                InteractorFactory.GetAutocompleteSuggestions(
                    Arg.Any<QueryInfo>(),
                    Arg.Any<DefaultTimeEntrySearchEngine>())
                    .Execute()
                    .Returns(suggestions);

                ViewModel.Initialize(DefaultParameter);

                ViewModel.OnTextFieldInfoFromView(
                    new QueryTextSpan(description, description.Length),
                    new ProjectSpan(ProjectId, ProjectName, ProjectColor)
                );
                TestScheduler.Start();

                var result = observer.LastEmittedValue().ToList();
                result.Should().HaveCount(1);
                result[0].Items.Should().HaveCount(1);
                var suggestion = result[0].Items[0];
                suggestion.Should().BeOfType<TimeEntrySuggestion>();
                ((TimeEntrySuggestion)suggestion).ProjectId.Should().Be(ProjectId);
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
                InteractorFactory.GetAutocompleteSuggestions(
                    Arg.Any<QueryInfo>(),
                    Arg.Any<DefaultTimeEntrySearchEngine>())
                    .Execute()
                    .Returns(suggestionsObservable);

                var observer = TestScheduler
                    .CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                TestScheduler.Start();

                var result = observer.LastEmittedValue().ToList();
                result.Should().HaveCount(2);
                projectNames(result[0].Items).Should().BeInAscendingOrder();
                projectNames(result[1].Items).Should().BeInAscendingOrder();
            }

            public async Task SortsTasksByName()
            {
                var suggestions = new List<ProjectSuggestion>();
                suggestions.Add(getProjectSuggestion(3, 0, new[]
                {
                    new MockTask { Id = 2, WorkspaceId = 0, ProjectId = 3, Name = "Task2" },
                    new MockTask { Id = 1, WorkspaceId = 0, ProjectId = 3, Name = "Task1" },
                    new MockTask { Id = 3, WorkspaceId = 0, ProjectId = 3, Name = "Task3" }
                }));
                var suggestionsObservable = Observable.Return(suggestions);
                InteractorFactory.GetAutocompleteSuggestions(
                    Arg.Any<QueryInfo>(),
                    Arg.Any<DefaultTimeEntrySearchEngine>())
                    .Execute()
                    .Returns(suggestionsObservable);

                var observer = TestScheduler
                    .CreateObserver<IEnumerable<SectionModel<string, AutocompleteSuggestion>>>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);

                TestScheduler.Start();

                var result = observer.LastEmittedValue().ToList();
                ViewModel.ToggleTasks.Execute((ProjectSuggestion)result[0].Items[1]);
                TestScheduler.Start();

                result = observer.LastEmittedValue().ToList();
                result.Should().HaveCount(1);
                result[0].Items.Should().HaveCount(5);
                tasksNames(result[0].Items.Skip(2)).Should().BeInAscendingOrder();
            }
        }

        public sealed class TheTextFieldInfoProperty : StartTimeEntryViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData("abc @def")]
            [InlineData("abc #def")]
            public async Task DoesNotChangeSuggestionsWhenOnlyTheCursorMovesForward(string text)
            {
                var searchEngine = Substitute.For<ISearchEngine<IThreadSafeTimeEntry>>();
                var interactor = Substitute.For<GetAutocompleteSuggestions>(InteractorFactory, new QueryInfo("", AutocompleteSuggestionType.None), searchEngine);
                InteractorFactory.GetAutocompleteSuggestions(
                    Arg.Any<QueryInfo>(),
                    Arg.Any<DefaultTimeEntrySearchEngine>())
                    .Returns(interactor);

                ViewModel.Initialize(DefaultParameter).Wait();
                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan(text, text.Length));

                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan(text, 0));

                TestScheduler.Start();
                await interactor.DidNotReceive().Execute();
            }

            [Theory, LogIfTooSlow]
            [InlineData("abc @def")]
            [InlineData("abc #def")]
            public async Task ChangesSuggestionsWhenTheCursorMovesBackBehindTheOldCursorPosition(string text)
            {
                var searchEngine = Substitute.For<ISearchEngine<IThreadSafeTimeEntry>>();
                var interactor = Substitute.For<GetAutocompleteSuggestions>(InteractorFactory, new QueryInfo("", AutocompleteSuggestionType.None), searchEngine);
                InteractorFactory.GetAutocompleteSuggestions(
                    Arg.Any<QueryInfo>(),
                    Arg.Any<DefaultTimeEntrySearchEngine>())
                    .Returns(interactor);

                ViewModel.Suggestions.Subscribe();

                var extendedText = text + "x";
                ViewModel.Initialize(DefaultParameter).Wait();
                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan(extendedText, text.Length));
                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan(extendedText, 0));

                ViewModel.OnTextFieldInfoFromView(new QueryTextSpan(extendedText, extendedText.Length));
                TestScheduler.Start();

                await interactor.Received().Execute();
            }

            [Theory, LogIfTooSlow]
            [InlineData("abc @def")]
            [InlineData("abc #def")]
            public async Task ChangesSuggestionsWhenTheCursorMovesBeforeTheQuerySymbolAndUserStartsTyping(string text)
            {
                var searchEngine = Substitute.For<ISearchEngine<IThreadSafeTimeEntry>>();
                var interactor = Substitute.For<GetAutocompleteSuggestions>(InteractorFactory, new QueryInfo("", AutocompleteSuggestionType.None), searchEngine);
                InteractorFactory.GetAutocompleteSuggestions(
                    Arg.Is<QueryInfo>(query => query.Text.StartsWith("x")),
                    Arg.Any<DefaultTimeEntrySearchEngine>())
                    .Returns(interactor);

                ViewModel.Suggestions.Subscribe();

                ViewModel.Initialize(DefaultParameter).Wait();
                ViewModel.SetTextSpans(ImmutableList.Create<ISpan>(new QueryTextSpan(text, text.Length)));
                ViewModel.SetTextSpans(ImmutableList.Create<ISpan>(new QueryTextSpan(text, 0)));

                TestScheduler.Start();

                ViewModel.SetTextSpans(ImmutableList.Create<ISpan>(new QueryTextSpan("x" + text, 1)));

                TestScheduler.Start();
                await interactor.Received().Execute();
            }
        }

        public sealed class TheNoTagsInfoMessage : StartTimeEntryViewModelTest
        {

            [Theory, LogIfTooSlow]
            [InlineData("")]
            [InlineData("asd ")]
            [InlineData("\tasd asd ")]
            [InlineData("x")]
            public async Task AppearsWhenSuggestingTagsAndUserHasNoTags(string query)
            {
                var observer = TestScheduler.CreateObserver<CollectionSections>();
                ViewModel.Suggestions.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameter);
                ViewModel.OnTextFieldInfoFromView(
                    new QueryTextSpan($"{QuerySymbols.Tags}{query}", 1)
                );

                TestScheduler.Start();
                var items = observer.LastEmittedValue().SelectMany(s => s.Items);
                var noEntity = items.Where(i => i is NoEntityInfoMessage).Select(i => i as NoEntityInfoMessage);
                noEntity.Should().Contain(i => i.Text == Resources.NoTagsInfoMessage);
            }

            [Theory, LogIfTooSlow]
            [InlineData(1, " ")]
            [InlineData(2, "#tag")]
            [InlineData(8, "@Project")]
            [InlineData(3, "Time entry")]
            [InlineData(1, "#")]
            public async Task DoesntAppearInAnyOtherCase(int tagCount, string query)
            {
                var observer = TestScheduler.CreateObserver<CollectionSections>();
                ViewModel.Suggestions.Subscribe(observer);

                var tags = Enumerable
                    .Range(0, tagCount)
                    .Select(_ => Substitute.For<IThreadSafeTag>());
                DataSource.Tags.GetAll().Returns(Observable.Return(tags));
                await ViewModel.Initialize(DefaultParameter);
                ViewModel.OnTextFieldInfoFromView(
                    new QueryTextSpan(query, 1)
                );

                TestScheduler.Start();
                var items = observer.LastEmittedValue().SelectMany(s => s.Items);
                var noEntity = items.Where(i => i is NoEntityInfoMessage).Select(i => i as NoEntityInfoMessage);
                noEntity.Should().NotContain(i => i.Text == Resources.NoTagsInfoMessage);
            }

            [Theory, LogIfTooSlow]
            [InlineData("")]
            [InlineData("asd ")]
            [InlineData("\tasd asd ")]
            [InlineData("x")]
            public async Task DoesntAppearAfterCreatingATag(string query)
            {
                var observer = TestScheduler.CreateObserver<CollectionSections>();
                ViewModel.Suggestions.Subscribe(observer);

                var tag = Substitute.For<IThreadSafeTag>();
                InteractorFactory.CreateTag(Arg.Any<string>(), Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(tag));
                await ViewModel.Initialize(DefaultParameter);

                ViewModel.SetTextSpans(
                        ImmutableList.Create<ISpan>(new QueryTextSpan($"{QuerySymbols.Tags}{query}", 1)));
                ViewModel.SelectSuggestion.ExecuteWithCompletion(new TagSuggestion(tag));

                TestScheduler.Start();
                var items = observer.LastEmittedValue().SelectMany(s => s.Items);
                var noEntity = items.Where(i => i is NoEntityInfoMessage).Select(i => i as NoEntityInfoMessage);
                noEntity.Should().NotContain(i => i.Text == Resources.NoTagsInfoMessage);
            }
        }
    }

    public static class TestExtensions
    {
        public static void OnTextFieldInfoFromView(this StartTimeEntryViewModel viewModel,params ISpan[] spans)
        {
            viewModel.SetTextSpans(spans.ToImmutableList());
        }

        public static QueryTextSpan GetQuerySpan(this TextFieldInfo textFieldInfo)
            => textFieldInfo.Spans.OfType<QueryTextSpan>().Single();

        public static ProjectSpan GetProjectSpan(this TextFieldInfo textFieldInfo)
            => textFieldInfo.Spans.OfType<ProjectSpan>().Single();

        public static TagSpan GetTagSpan(this TextFieldInfo textFieldInfo)
            => textFieldInfo.Spans.OfType<TagSpan>().Single();

        public static TextSpan FirstTextSpan(this TextFieldInfo textFieldInfo)
            => textFieldInfo.Spans.OfType<TextSpan>().First();
    }
}
