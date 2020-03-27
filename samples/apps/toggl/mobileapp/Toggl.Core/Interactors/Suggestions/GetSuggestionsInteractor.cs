using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Suggestions;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Interactors.Suggestions
{
    public sealed class GetSuggestionsInteractor : IInteractor<IObservable<IEnumerable<Suggestion>>>
    {
        private readonly int suggestionCount;
        private readonly IInteractor<IObservable<IReadOnlyList<ISuggestionProvider>>> getSuggestionProvidersInteractor;

        public GetSuggestionsInteractor(
            int suggestionCount,
            IInteractorFactory interactorFactory)
        {
            Ensure.Argument.IsInClosedRange(suggestionCount, 1, 9, nameof(suggestionCount));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.getSuggestionProvidersInteractor = interactorFactory.GetSuggestionProviders(suggestionCount);
            this.suggestionCount = suggestionCount;
        }

        public IObservable<IEnumerable<Suggestion>> Execute()
            => getSuggestionProvidersInteractor
                .Execute()
                .Flatten()
                .Select(provider => provider.GetSuggestions())
                .Flatten()
                .ToList()
                .Select(removingDuplicates)
                .Select(suggestions => suggestions.Take(suggestionCount));

        private IList<Suggestion> removingDuplicates(IList<Suggestion> suggestions)
            => suggestions
                .Distinct(new SuggestionsComparer())
                .ToList();

        private sealed class SuggestionsComparer : IEqualityComparer<Suggestion>
        {
            public bool Equals(Suggestion s1, Suggestion s2)
                => s1 != null
                   && s2 != null
                   && s1.WorkspaceId == s2.WorkspaceId
                   && s1.Description == s2.Description
                   && s1.ProjectId == s2.ProjectId
                   && s1.TaskId == s2.TaskId;

            public int GetHashCode(Suggestion suggestion)
                => HashCode.Combine(
                    suggestion.WorkspaceId,
                    suggestion.Description,
                    suggestion.ProjectId,
                    suggestion.TaskId);
        }
    }
}
