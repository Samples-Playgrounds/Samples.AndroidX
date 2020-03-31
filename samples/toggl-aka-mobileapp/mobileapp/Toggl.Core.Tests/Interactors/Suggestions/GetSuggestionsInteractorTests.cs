using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Toggl.Core.Interactors;
using Toggl.Core.Interactors.Suggestions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Suggestions;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Xunit;

namespace Toggl.Core.Tests.Interactors.Suggestions
{
    public sealed class GetSuggestionsInteractorTests
    {
        public sealed class TheConstructor : BaseInteractorTests
        {
            [Xunit.Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useInteractorFactory)
            {
                Action createInstance = () => new GetSuggestionsInteractor(
                    3,
                    useInteractorFactory ? InteractorFactory : null);

                createInstance.Should().Throw<ArgumentNullException>();
            }

            [Xunit.Theory, LogIfTooSlow]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(10)]
            [InlineData(-100)]
            [InlineData(256)]
            public void ThrowsIfTheCountIsOutOfRange(int count)
            {
                Action createInstance = () => new GetSuggestionsInteractor(
                    count,
                    InteractorFactory);

                createInstance.Should().Throw<ArgumentException>();
            }
        }

        public sealed class TheExecuteMethod : BaseInteractorTests
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsSuggestionsWithoutDuplicates()
            {
                var randomProvider = makeProvider(new List<Suggestion>
                {
                    makeSuggestion("same description", 12, SuggestionProviderType.MostUsedTimeEntries)
                });

                var suggestionProvidersObservable = Observable.Return(new List<ISuggestionProvider>
                {
                    randomProvider,
                    randomProvider,
                    randomProvider
                });

                var interactorFactory = Substitute.For<IInteractorFactory>();
                interactorFactory.GetSuggestionProviders(3).Execute()
                    .Returns(suggestionProvidersObservable);

                var interactor = new GetSuggestionsInteractor(3, interactorFactory);

                var suggestions = await interactor.Execute();
                suggestions.Should().HaveCount(1);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsSuggestionsWithoutRemoveingNonDuplicates()
            {
                var randomProvider1 = makeProvider(new List<Suggestion>
                {
                    makeSuggestion("1 description", 11, SuggestionProviderType.RandomForest)
                });

                var randomProvider2 = makeProvider(new List<Suggestion>
                {
                    makeSuggestion("2 description", 12, SuggestionProviderType.MostUsedTimeEntries)
                });

                var randomProvider3 = makeProvider(new List<Suggestion>
                {
                    makeSuggestion("3 description", 13, SuggestionProviderType.Calendar)
                });

                var suggestionProvidersObservable = Observable.Return(new List<ISuggestionProvider>
                {
                    randomProvider1,
                    randomProvider2,
                    randomProvider3
                });

                var interactorFactory = Substitute.For<IInteractorFactory>();
                interactorFactory.GetSuggestionProviders(3).Execute()
                    .Returns(suggestionProvidersObservable);

                var interactor = new GetSuggestionsInteractor(3, interactorFactory);

                var suggestions = await interactor.Execute();
                suggestions.Should().HaveCount(3);
            }

            [Xunit.Theory, LogIfTooSlow]
            [InlineData(3, 3)]
            [InlineData(2, 1)]
            [InlineData(5, 1)]
            [InlineData(1, 5)]
            public async Task ReturnsSuggestionsWithCountEqualOrLessThanRequested(int numberOfProviders, int numberofSuggestionsPerProvider)
            {
                var suggestionProvidersObservable = Observable.Return(makeCombinationProvidersAndSuggestions(numberOfProviders, numberofSuggestionsPerProvider));

                var interactorFactory = Substitute.For<IInteractorFactory>();
                interactorFactory.GetSuggestionProviders(3).Execute()
                    .Returns(suggestionProvidersObservable);

                var interactor = new GetSuggestionsInteractor(3, interactorFactory);

                var suggestions = await interactor.Execute();
                suggestions.Should().HaveCountLessOrEqualTo(3);
            }

            private Suggestion makeSuggestion(string description, long? projectId, SuggestionProviderType type)
                => new Suggestion(new MockTimeEntry
                {
                    Description = description,
                    ProjectId = projectId
                }, type);

            private ISuggestionProvider makeProvider(List<Suggestion> suggestions)
            {
                var provider = Substitute.For<ISuggestionProvider>();
                provider.GetSuggestions().Returns(suggestions.ToObservable());
                return provider;
            }

            private List<ISuggestionProvider> makeCombinationProvidersAndSuggestions(int numberOfProviders, int numberofSuggestionsPerProvider)
            {
                var providers = new List<ISuggestionProvider>();
                for (int i = 0; i < numberOfProviders; i++)
                {
                    var suggestions = new List<Suggestion>();

                    for (int j = 0; j < numberofSuggestionsPerProvider; j++)
                    {
                        suggestions.Add(makeSuggestion($"{i}{j}", i * 1000 + j, SuggestionProviderType.MostUsedTimeEntries));
                    }

                    providers.Add(makeProvider(suggestions));
                }

                return providers;
            }
        }
    }
}
