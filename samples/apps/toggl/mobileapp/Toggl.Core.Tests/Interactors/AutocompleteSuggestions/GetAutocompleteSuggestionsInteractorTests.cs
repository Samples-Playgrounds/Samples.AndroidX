using NSubstitute;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Autocomplete;
using Toggl.Core.Autocomplete.Span;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Interactors.AutocompleteSuggestions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Search;
using Toggl.Core.Tests.Autocomplete;
using Xunit;

namespace Toggl.Core.Tests.Interactors.AutocompleteSuggestions
{
    public sealed class AutocompleteProviderTests
    {
        public class GetAutocompleteSuggestionsInteractorTests : BaseInteractorTests
        {
            protected const long WorkspaceId = 9;
            protected const long ProjectId = 10;
            protected const string ProjectName = "Toggl";
            protected const string ProjectColor = "#F41F19";

            protected IInteractorFactory InteractorFactory { get; } = Substitute.For<IInteractorFactory>();

            private ISearchEngine<IThreadSafeTimeEntry> setupMockSearchEngine(ImmutableList<IThreadSafeTimeEntry> timeEntries = null)
            {
                var engine = Substitute.For<ISearchEngine<IThreadSafeTimeEntry>>();
                engine.SetInitialData(timeEntries);
                return engine;
            }

            [Theory, LogIfTooSlow]
            [InlineData("Nothing")]
            [InlineData("Testing Toggl mobile apps")]
            public async Task WhenTheUserBeginsTypingADescription(string description)
            {
                var textFieldInfo = TextFieldInfo.Empty(1)
                    .ReplaceSpans(new QueryTextSpan(description, 0));
                var searchEngine = setupMockSearchEngine();

                var interactor = new GetAutocompleteSuggestions(InteractorFactory, QueryInfo.ParseFieldInfo(textFieldInfo), searchEngine);
                await interactor.Execute();

                await searchEngine.Received().Get(Arg.Is(description));
            }

            [Theory, LogIfTooSlow]
            [InlineData("Nothing")]
            [InlineData("Testing Toggl mobile apps")]
            public async Task WhenTheUserHasTypedAnySearchSymbolsButMovedTheCaretToAnIndexThatComesBeforeTheSymbol(
                string description)
            {
                var actualDescription = $"{description} @{description}";
                var textFieldInfo = TextFieldInfo.Empty(1)
                    .ReplaceSpans(new QueryTextSpan(actualDescription, 0));
                var searchEngine = setupMockSearchEngine();

                var interactor = new GetAutocompleteSuggestions(InteractorFactory, QueryInfo.ParseFieldInfo(textFieldInfo), searchEngine);
                await interactor.Execute();

                await searchEngine.Received().Get(Arg.Is(actualDescription));
            }

            [Fact, LogIfTooSlow]
            public async Task WhenTheUserHasAlreadySelectedAProjectAndTypesTheAtSymbol()
            {
                var description = $"Testing Mobile Apps @toggl";
                var textFieldInfo = TextFieldInfo.Empty(1).ReplaceSpans(
                    new QueryTextSpan(description, description.Length),
                    new ProjectSpan(ProjectId, ProjectName, ProjectColor)
                );
                var searchEngine = setupMockSearchEngine();

                var interactor = new GetAutocompleteSuggestions(InteractorFactory, QueryInfo.ParseFieldInfo(textFieldInfo), searchEngine);
                await interactor.Execute();

                await searchEngine.Received().Get(Arg.Is(description));
            }

            [Theory, LogIfTooSlow]
            [InlineData("Nothing")]
            [InlineData("Testing Toggl mobile apps")]
            public async Task UsesGetProjectsAutocompleteSuggestionsInteractorWhenTheAtSymbolIsTyped(string description)
            {
                var actualDescription = $"{description} @{description}";
                var textFieldInfo = TextFieldInfo.Empty(1)
                    .ReplaceSpans(new QueryTextSpan(actualDescription, description.Length + 2));
                var searchEngine = setupMockSearchEngine();

                var interactor = new GetAutocompleteSuggestions(InteractorFactory, QueryInfo.ParseFieldInfo(textFieldInfo), searchEngine);
                await interactor.Execute();

                InteractorFactory
                    .Received()
                    .GetProjectsAutocompleteSuggestions(Arg.Is<IList<string>>(
                        words => words.SequenceEqual(description.SplitToQueryWords())));
            }

            [Theory, LogIfTooSlow]
            [InlineData("Nothing")]
            [InlineData("Testing Toggl mobile apps")]
            public async Task UsesGetTagsAutocompleteSuggestionsInteractorWhenTheHashtagSymbolIsTyped(string description)
            {
                var actualDescription = $"{description} #{description}";
                var textFieldInfo = TextFieldInfo.Empty(1)
                    .ReplaceSpans(new QueryTextSpan(actualDescription, description.Length + 2));
                var searchEngine = setupMockSearchEngine();

                var interactor = new GetAutocompleteSuggestions(InteractorFactory, QueryInfo.ParseFieldInfo(textFieldInfo), searchEngine);
                await interactor.Execute();

                InteractorFactory
                    .Received()
                    .GetTagsAutocompleteSuggestions(Arg.Is<IList<string>>(
                        words => words.SequenceEqual(description.SplitToQueryWords())));
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotUseInteractorsWhenTheSarchStringIsEmpty()
            {
                var textFieldInfo = TextFieldInfo.Empty(1).ReplaceSpans(new QueryTextSpan());
                var searchEngine = setupMockSearchEngine();

                var interactor = new GetAutocompleteSuggestions(InteractorFactory, QueryInfo.ParseFieldInfo(textFieldInfo), searchEngine);
                await interactor.Execute();

                InteractorFactory
                    .DidNotReceive()
                    .GetTagsAutocompleteSuggestions(Arg.Any<IList<string>>());
                InteractorFactory
                    .DidNotReceive()
                    .GetProjectsAutocompleteSuggestions(Arg.Any<IList<string>>());
                await searchEngine.DidNotReceive().Get(Arg.Any<string>());
            }
        }
    }
}
