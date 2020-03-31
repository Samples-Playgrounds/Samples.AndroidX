using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Extensions;

namespace Toggl.Core.Search
{
    internal class SearchEngineCacheItem<T>
    {
        public string Query { get; }
        public ImmutableList<T> Items { get; private set; }

        public SearchEngineCacheItem(string query, ImmutableList<T> originalItems)
        {
            Query = query;
            Items = originalItems;
        }

        public SearchEngineCacheItem<T> Filter(string query, Func<string, T, bool> filter)
        {
            var words = query.SplitToQueryWords();

            var filteredData = words
                .Aggregate(Items as IEnumerable<T>, (collection, word) => applyFilter(collection, word, filter))
                .ToImmutableList();

            return new SearchEngineCacheItem<T>(query, filteredData);
        }

        private IEnumerable<T> applyFilter(IEnumerable<T> collection, string word, Func<string, T, bool> filter)
            => collection.Where(item => filter(word, item));
    }
}
