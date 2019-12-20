using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Core.Extensions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class SelectTagsViewModelTests
    {
        public abstract class SelectTagsViewModelTest : BaseViewModelTests<SelectTagsViewModel, SelectTagsParameter, long[]>
        {
            protected SelectTagsParameter DefaultParameter { get; } = new SelectTagsParameter(new long[0], 1);

            protected override SelectTagsViewModel CreateViewModel()
                => new SelectTagsViewModel(
                    NavigationService,
                    InteractorFactory,
                    SchedulerProvider,
                    RxActionFactory
                );

            protected void EnsureClosesTheViewModel()
            {
                View.Received().Close();
            }

            protected bool EnsureExpectedTagsAreReturned(long[] actual, long[] expected)
            {
                if (actual.Length != expected.Length) return false;

                foreach (var actualTag in actual)
                {
                    if (!expected.Contains(actualTag))
                        return false;
                }
                return true;
            }

            protected IEnumerable<TagSuggestion> CreateTags(int count)
                => Enumerable
                    .Range(0, count)
                    .Select(i => CreateTagSubstitute(i, i.ToString()))
                    .Select(tag => new TagSuggestion(tag));

            protected IThreadSafeTag CreateTagSubstitute(long id, string name)
            {
                var tag = Substitute.For<IThreadSafeTag>();
                tag.Id.Returns(id);
                tag.Name.Returns(name);
                return tag;
            }
        }

        public sealed class TheConstructor : SelectTagsViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useNavigationService,
                bool useInteractorFactory,
                bool useRxActionFactory,
                bool useSchedulerProvider)
            {
                var navigationService = useNavigationService ? NavigationService : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new SelectTagsViewModel(
                        navigationService,
                        interactorFactory,
                        schedulerProvider,
                        rxActionFactory);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheCloseWithDefaultResultMethod : SelectTagsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.Initialize(DefaultParameter);

                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                EnsureClosesTheViewModel();
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheSameTagsThatWerePassedToTheViewModel()
            {
                var tagids = new long[] { 1, 4, 29, 2 };
                await ViewModel.Initialize(new SelectTagsParameter(tagids, 0));

                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                (await ViewModel.Result).Should().BeSequenceEquivalentTo(tagids);
            }
        }

        public sealed class TheSaveCommand : SelectTagsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.Initialize(DefaultParameter);

                ViewModel.Save.Execute();
                TestScheduler.Start();

                EnsureClosesTheViewModel();
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheSelectedTagIds()
            {
                await ViewModel.Initialize(DefaultParameter);
                var tagIds = Enumerable.Range(0, 20).Select(num => (long)num);
                var selectedTagIds = tagIds.Where(id => id % 2 == 0)
                    .ToArray();
                var selectedTags = selectedTagIds
                    .Select(createDatabaseTagSubstitute)
                    .Select(databaseTag => new TagSuggestion(databaseTag))
                    .Select(tagSuggestion
                        => new SelectableTagViewModel(tagSuggestion.TagId, tagSuggestion.Name, false,
                            tagSuggestion.WorkspaceId));

                ViewModel.SelectTag.ExecuteSequentally(selectedTags)
                    .PrependAction(ViewModel.Save)
                    .Subscribe();

                TestScheduler.Start();

                var ids = await ViewModel.Result;
                EnsureExpectedTagsAreReturned(ids, selectedTagIds);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsEmptyArrayIfNoTagsWereSelected()
            {
                await ViewModel.Initialize(DefaultParameter);
                var expectedIds = new long[0];

                ViewModel.Save.Execute();

                var ids = await ViewModel.Result;
                EnsureExpectedTagsAreReturned(ids, expectedIds);
            }

            private IThreadSafeTag createDatabaseTagSubstitute(long id)
            {
                var tag = Substitute.For<IThreadSafeTag>();
                tag.Id.Returns(id);
                tag.Name.Returns($"Tag{id}");
                return tag;
            }
        }

        public sealed class TheFilterTextProperty : SelectTagsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task WhenChangedQueriesTheAutocompleteProvider()
            {
                var text = "Some text";
                await ViewModel.Initialize(DefaultParameter);
                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableTagBaseViewModel>>();
                ViewModel.Tags.Subscribe(observer);

                ViewModel.FilterText.OnNext(text);
                TestScheduler.Start();

                InteractorFactory.Received()
                    .GetTagsAutocompleteSuggestions(
                        Arg.Is<IList<string>>(words => words.SequenceEqual(text.SplitToQueryWords())));
            }
        }

        public sealed class TheIsEmptyProperty : SelectTagsViewModelTest
        {
            private const long workspaceId = 1;
            private const long irrelevantWorkspaceId = 2;

            private void setup(Func<long, long> workspaceIdSelector)
            {
                var tags = Enumerable.Range(0, 10)
                                     .Select(i =>
                                     {
                                         var tag = Substitute.For<IThreadSafeTag>();
                                         tag.Name.Returns(Guid.NewGuid().ToString());
                                         tag.Id.Returns(i);
                                         tag.WorkspaceId.Returns(workspaceIdSelector(i));
                                         return tag;
                                     })
                                     .ToList();

                InteractorFactory
                    .GetTagsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(Observable.Return(TagSuggestion.FromTags(tags)));
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTrueIfHasNoTagsForSelectedWorkspace()
            {
                setup(i => irrelevantWorkspaceId);

                await ViewModel.Initialize(new SelectTagsParameter(new long[] { }, workspaceId));

                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsEmpty.Subscribe(observer);

                TestScheduler.Start();

                observer.LastEmittedValue().Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsFalseIfTagsForWorkspaceExist()
            {
                setup(i => i % 2 == 0 ? irrelevantWorkspaceId : workspaceId);

                await ViewModel.Initialize(new SelectTagsParameter(new long[] { }, workspaceId));

                var observer = TestScheduler.CreateObserver<bool>();

                ViewModel.IsEmpty.Subscribe(observer);

                TestScheduler.Start();

                observer.LastEmittedValue().Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsFalseIfTagsForWorkspaceExistButFilteredCollectionIsEmpty()
            {
                setup(i => i % 2 == 0 ? irrelevantWorkspaceId : workspaceId);

                await ViewModel.Initialize(new SelectTagsParameter(new long[] { }, workspaceId));

                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsEmpty.Subscribe(observer);
                ViewModel.FilterText.OnNext("Anything");
                TestScheduler.Start();

                observer.LastEmittedValue().Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsFalseIfTagIsCreated()
            {
                InteractorFactory
                    .GetTagsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(Observable.Return(new List<AutocompleteSuggestion>()));

                var newTag = Substitute.For<IThreadSafeTag>();
                newTag.Id.Returns(12345);
                newTag.WorkspaceId.Returns(workspaceId);
                newTag.Name.Returns("new tag");

                await ViewModel.Initialize(new SelectTagsParameter(new long[] { }, workspaceId));

                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsEmpty.Subscribe(observer);
                InteractorFactory
                    .GetTagsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(Observable.Return(TagSuggestion.FromTags(new List<IThreadSafeTag> { newTag })));
                ViewModel.FilterText.OnNext(string.Empty);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().BeFalse();
            }
        }

        public sealed class TheTagsProperty : SelectTagsViewModelTest
        {
            private IEnumerable<TagSuggestion> getTagSuggestions(int count, IThreadSafeWorkspace workspace)
            {
                for (int i = 0; i < count; i++)
                {
                    /* Do not inline 'workspace.Id' into another .Return() call
                     * because it's a proxy that won't work later on!
                     * This must be cached before usage.
                     */
                    var workspaceId = workspace.Id;

                    var tag = Substitute.For<IThreadSafeTag>();
                    tag.Id.Returns(i);
                    tag.WorkspaceId.Returns(workspaceId);
                    tag.Workspace.Returns(workspace);
                    tag.Name.Returns($"Tag{i}");

                    yield return new TagSuggestion(tag);
                }
            }

            private IThreadSafeWorkspace createWorkspace(long id, string name)
            {
                var workspace = Substitute.For<IThreadSafeWorkspace>();
                workspace.Id.Returns(id);
                workspace.Name.Returns(name);
                return workspace;
            }

            [Fact, LogIfTooSlow]
            public async Task OnlyContainsTagsFromTheSameWorkspaceAsTimeEntry()
            {
                var tags = new List<TagSuggestion>();
                var workspaces = Enumerable.Range(0, 5)
                    .Select(i => createWorkspace(i, $"Workspace{i}")).ToArray();
                workspaces.ForEach(workspace
                    => tags.AddRange(getTagSuggestions(5, workspace)));
                InteractorFactory.GetTagsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(Observable.Return(tags));
                var targetWorkspace = workspaces[1];
                InteractorFactory.GetWorkspaceById(targetWorkspace.Id).Execute()
                    .Returns(Observable.Return(targetWorkspace));
                var tagIds = tags.Select(tag => tag.TagId).ToArray();

                await ViewModel.Initialize(new SelectTagsParameter(tagIds, targetWorkspace.Id));

                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableTagBaseViewModel>>();
                ViewModel.Tags.Subscribe(observer);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().HaveCount(5);
                observer.LastEmittedValue().Should()
                    .OnlyContain(tag => tag.WorkspaceId == targetWorkspace.Id);
            }

            [Fact, LogIfTooSlow]
            public async Task IsPopulatedAfterInitialization()
            {
                var workspace = createWorkspace(13, "Some workspace");
                var tagSuggestions = getTagSuggestions(10, workspace);
                var tagIds = tagSuggestions.Select(tag => tag.TagId).ToArray();
                InteractorFactory.GetTagsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(Observable.Return(tagSuggestions));
                InteractorFactory.GetWorkspaceById(workspace.Id).Execute()
                    .Returns(Observable.Return(workspace));

                await ViewModel.Initialize(new SelectTagsParameter(tagIds, workspace.Id));

                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableTagBaseViewModel>>();
                ViewModel.Tags.Subscribe(observer);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().HaveCount(tagSuggestions.Count());
            }

            [Fact, LogIfTooSlow]
            public async Task IsSortedBySelectedStatusThenByName()
            {
                var workspace = createWorkspace(13, "Some workspace");
                var tagSuggestions = getTagSuggestions(4, workspace).ToArray();

                var shuffledTags = new[] { tagSuggestions[3], tagSuggestions[1], tagSuggestions[2], tagSuggestions[0] };
                var selectedTagIds = new[] { tagSuggestions[0].TagId, tagSuggestions[2].TagId };
                InteractorFactory.GetTagsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(Observable.Return(shuffledTags));
                InteractorFactory.GetWorkspaceById(workspace.Id).Execute()
                    .Returns(Observable.Return(workspace));

                await ViewModel.Initialize(new SelectTagsParameter(selectedTagIds, workspace.Id));

                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableTagBaseViewModel>>();
                ViewModel.Tags.Subscribe(observer);
                TestScheduler.Start();

                var tags = observer.LastEmittedValue().ToArray();
                tags.Should().HaveCount(4);


                tags[0].Name.Should().Be("Tag0");
                tags[1].Name.Should().Be("Tag2");
                tags[2].Name.Should().Be("Tag1");
                tags[3].Name.Should().Be("Tag3");

                tags[0].Selected.Should().BeTrue();
                tags[1].Selected.Should().BeTrue();
                tags[2].Selected.Should().BeFalse();
                tags[3].Selected.Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public async Task IsUpdatedWhenTextIsChanged()
            {
                var workspace = createWorkspace(13, "Some workspace");
                var oldSuggestions = getTagSuggestions(3, workspace);
                var newSuggestions = getTagSuggestions(1, workspace);
                var oldTagIds = oldSuggestions.Select(tag => tag.TagId).ToArray();
                var queryText = "Query text";
                InteractorFactory.GetTagsAutocompleteSuggestions(
                        Arg.Is<IList<string>>(words => words.SequenceEqual(queryText.SplitToQueryWords())))
                    .Execute()
                    .Returns(Observable.Return(newSuggestions));
                InteractorFactory.GetWorkspaceById(workspace.Id).Execute()
                    .Returns(Observable.Return(workspace));

                await ViewModel.Initialize(new SelectTagsParameter(oldTagIds, workspace.Id));

                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableTagBaseViewModel>>();
                ViewModel.Tags.Subscribe(observer);
                ViewModel.FilterText.OnNext(queryText);
                TestScheduler.Start();

                var received = observer.LastEmittedValue();
                received.Should().HaveCount(2);
            }

            [Fact, LogIfTooSlow]
            public async Task AddCreationModelWhenNoMatchingSuggestion()
            {
                var workspace = createWorkspace(13, "Some workspace");
                var tagSuggestions = getTagSuggestions(10, workspace);
                InteractorFactory.GetTagsAutocompleteSuggestions(Arg.Any<IList<string>>())
                    .Execute()
                    .Returns(Observable.Return(tagSuggestions));
                InteractorFactory.GetWorkspaceById(workspace.Id).Execute()
                    .Returns(Observable.Return(workspace));
                var tagIds = tagSuggestions.Select(tag => tag.TagId).ToArray();

                var nonExistingTag = "Non existing tag";

                await ViewModel.Initialize(new SelectTagsParameter(tagIds, workspace.Id));

                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableTagBaseViewModel>>();

                ViewModel.Tags.Subscribe(observer);
                ViewModel.FilterText.OnNext(nonExistingTag);
                TestScheduler.Start();

                observer.LastEmittedValue().First().Name.Should().Be(nonExistingTag);
                observer.LastEmittedValue().First().WorkspaceId.Should().Be(workspace.Id);
                observer.LastEmittedValue().First().Selected.Should().BeFalse();
                observer.LastEmittedValue().First().Should().BeOfType<SelectableTagCreationViewModel>();
            }
        }

        public sealed class TheSelectTagAction : SelectTagsViewModelTest
        {
            private TagSuggestion tagSuggestion;

            public TheSelectTagAction()
            {
                var databaseTag = Substitute.For<IThreadSafeTag>();
                databaseTag.Name.Returns("Tag0");
                databaseTag.Id.Returns(0);
                tagSuggestion = new TagSuggestion(databaseTag);
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesANewTagWithTheGivenNameInTheCurrentWorkspace()
            {
                long workspaceId = 10;

                var newTag = new SelectableTagCreationViewModel("Some tag", workspaceId);
                await ViewModel.Initialize(DefaultParameter);

                ViewModel.SelectTag.Execute(newTag);
                TestScheduler.Start();

                await InteractorFactory
                    .Received()
                    .CreateTag(Arg.Is(newTag.Name), Arg.Is(workspaceId))
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task AppendsTheTagIdToSelectedTagIdsIfNotSelectedAlready()
            {
                var selectableTag = new SelectableTagViewModel(tagSuggestion.TagId, tagSuggestion.Name, true, 1);

                ViewModel.SelectTag.Execute(selectableTag);
                ViewModel.Save.Execute();
                TestScheduler.Start();

                var ids = await ViewModel.Result;
                EnsureExpectedTagsAreReturned(ids, new[] { selectableTag.Id });
            }

            [Fact, LogIfTooSlow]
            public async Task RemovesTheTagIdFromSelectedTagIdsIfSelectedAlready()
            {
                var selectableTag = new SelectableTagViewModel(tagSuggestion.TagId, tagSuggestion.Name, true, 1);

                await ViewModel.Initialize(new SelectTagsParameter(new long[] { selectableTag.Id }, 0));

                ViewModel.SelectTag.Execute(selectableTag);
                ViewModel.Save.Execute();
                TestScheduler.Start();

                var ids = await ViewModel.Result;
                EnsureExpectedTagsAreReturned(ids, new long[0]);
            }
        }

        public sealed class ThePrepareMethod : SelectTagsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task AddsAllPassedTagsToTheSelectedTags()
            {
                var tagIds = new long[] { 100, 3, 10, 34, 532 };

                await ViewModel.Initialize(new SelectTagsParameter(tagIds, 0));

                ViewModel.Save.Execute();

                var ids = await ViewModel.Result;
                EnsureExpectedTagsAreReturned(ids, tagIds);
            }
        }
    }
}

