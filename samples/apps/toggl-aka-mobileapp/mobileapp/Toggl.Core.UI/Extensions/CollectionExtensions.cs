using System.Collections.Generic;
using System.Collections.Immutable;

namespace Toggl.Core.UI.Extensions
{
    public static class CollectionExtensions
    {
        public static IImmutableList<T> ToIImmutableList<T>(this IEnumerable<T> original)
            => original.ToImmutableList();
    }
}
