using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Toggl.Core.Search
{
    public interface ISearchEngine<T>
    {
        void SetInitialData(ImmutableList<T> initialData);
        Task<IEnumerable<T>> Get(string query);
    }
}
