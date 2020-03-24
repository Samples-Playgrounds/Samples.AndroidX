using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors.AutocompleteSuggestions
{
    internal sealed class GetTagsAutocompleteSuggestions : IInteractor<IObservable<IEnumerable<AutocompleteSuggestion>>>
    {
        private readonly IDataSource<IThreadSafeTag, IDatabaseTag> dataSource;

        private readonly IEnumerable<string> wordsToQuery;

        public GetTagsAutocompleteSuggestions(IDataSource<IThreadSafeTag, IDatabaseTag> dataSource, IEnumerable<string> wordsToQuery)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(wordsToQuery, nameof(wordsToQuery));

            this.dataSource = dataSource;
            this.wordsToQuery = wordsToQuery;
        }

        public IObservable<IEnumerable<AutocompleteSuggestion>> Execute()
            => wordsToQuery
                .Aggregate(dataSource.GetAll(), (obs, word) => obs.Select(filterByWord(word)))
                .Select(TagSuggestion.FromTags);

        private Func<IEnumerable<IThreadSafeTag>, IEnumerable<IThreadSafeTag>> filterByWord(string word)
            => tags => tags.Where(t => t.Name.ContainsIgnoringCase(word));
    }
}
