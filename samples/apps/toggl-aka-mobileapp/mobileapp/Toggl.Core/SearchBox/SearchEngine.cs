using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Extensions;

namespace Toggl.Core.Search
{
    public class SearchEngine<T> : ISearchEngine<T>
    {
        private readonly Dictionary<string, SearchEngineCacheItem<T>> cache = new Dictionary<string, SearchEngineCacheItem<T>>();
        private readonly Func<string, T, bool> filter;

        private TaskCompletionSource<bool> initialDataAvailable = new TaskCompletionSource<bool>();

        public SearchEngine(Func<string, T, bool> filter)
        {
            this.filter = filter;
        }

        public void SetInitialData(ImmutableList<T> initialData)
        {
            lock (cache)
            {
                cache[""] = new SearchEngineCacheItem<T>("", initialData);
            }

            initialDataAvailable.SetResult(true);
        }

        private SearchEngineCacheItem<T> determineBaseDataSource(string query, out bool exactQueryRetrievedFromCache)
        {
            lock (cache)
            {
                if (cache.TryGetValue(query, out var dataSource))
                {
                    exactQueryRetrievedFromCache = true;
                    return dataSource;
                }

                exactQueryRetrievedFromCache = false;

                return cache
                    .Where(cachedItem => cachedItem.Key.Length < query.Length)
                    .OrderByDescending(cachedItem => cachedItem.Key.Length)
                    .First(cachedItem => query.Contains(cachedItem.Key))
                    .Value;
            }

            throw new InvalidOperationException("The state of the search engine is invalid. This should never happen.");
        }

        public async Task<IEnumerable<T>> Get(string query)
        {
            await initialDataAvailable.Task;
            return await Task.Run(() => get(query).AsEnumerable());
        }

        private ImmutableList<T> get(string query)
        {
            var dataSource = determineBaseDataSource(query, out var exactQueryRetrievedFromCache);

            if (exactQueryRetrievedFromCache)
                return dataSource.Items;

            var filteredDataSource = dataSource.Filter(query, filter);

            lock (cache)
            {
                cache[query] = filteredDataSource;
            }

            return filteredDataSource.Items;
        }
    }
}
