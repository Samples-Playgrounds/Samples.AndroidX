using System.Collections.Generic;

namespace Toggl.Shared.Extensions
{
    public static class HashSetExtensions
    {
        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
        {
            foreach (var item in items)
                hashSet.Add(item);
        }
    }
}
