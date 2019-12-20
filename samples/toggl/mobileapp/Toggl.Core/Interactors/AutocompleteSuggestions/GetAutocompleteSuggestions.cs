using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Toggl.Core.Autocomplete;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Core.Extensions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Search;
using Toggl.Shared;

namespace Toggl.Core.Interactors.AutocompleteSuggestions
{
    public class GetAutocompleteSuggestions : IInteractor<IObservable<IEnumerable<AutocompleteSuggestion>>>
    {
        private readonly IInteractorFactory interactorFactory;
        private readonly QueryInfo queryInfo;
        private ISearchEngine<IThreadSafeTimeEntry> searchEngine;

        public GetAutocompleteSuggestions(IInteractorFactory interactorFactory, QueryInfo queryInfo, ISearchEngine<IThreadSafeTimeEntry> searchEngine)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(queryInfo, nameof(queryInfo));

            this.searchEngine = searchEngine;
            this.interactorFactory = interactorFactory;
            this.queryInfo = queryInfo;
        }

        public IObservable<IEnumerable<AutocompleteSuggestion>> Execute()
        {
            var wordsToQuery = queryInfo.Text.SplitToQueryWords();
            switch (queryInfo.SuggestionType)
            {
                case AutocompleteSuggestionType.Projects:
                    return interactorFactory.GetProjectsAutocompleteSuggestions(wordsToQuery).Execute();

                case AutocompleteSuggestionType.Tags:
                    return interactorFactory.GetTagsAutocompleteSuggestions(wordsToQuery).Execute();
            }

            if (wordsToQuery.Count == 0)
                return Observable.Return(QuerySymbolSuggestion.Suggestions);

            return searchEngine
                .Get(queryInfo.Text)
                .ToObservable()
                .Select(entries => TimeEntrySuggestion.FromTimeEntries(entries).Cast<AutocompleteSuggestion>());
        }
    }
}
