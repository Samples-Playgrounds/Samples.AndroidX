using System;
using System.Collections.Generic;
using Toggl.Core.Autocomplete;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Core.Interactors.AutocompleteSuggestions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Search;

namespace Toggl.Core.Interactors
{
    public partial class InteractorFactory
    {
        public IInteractor<IObservable<IEnumerable<AutocompleteSuggestion>>> GetAutocompleteSuggestions(QueryInfo queryInfo, ISearchEngine<IThreadSafeTimeEntry> searchEngine)
            => new GetAutocompleteSuggestions(this, queryInfo, searchEngine);

        public IInteractor<IObservable<IEnumerable<AutocompleteSuggestion>>> GetTagsAutocompleteSuggestions(IList<string> wordsToQuery)
            => new GetTagsAutocompleteSuggestions(dataSource.Tags, wordsToQuery);

        public IInteractor<IObservable<IEnumerable<AutocompleteSuggestion>>> GetProjectsAutocompleteSuggestions(IList<string> wordsToQuery)
            => new GetProjectsAutocompleteSuggestions(dataSource.Projects, wordsToQuery);
    }
}
