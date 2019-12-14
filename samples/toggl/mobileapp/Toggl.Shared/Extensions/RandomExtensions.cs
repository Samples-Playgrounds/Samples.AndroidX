using System;
using System.Collections.Generic;
using System.Threading;

namespace Toggl.Shared.Extensions
{
    public static class RandomExtensions
    {
        private static int seed = Environment.TickCount;

        private static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public static T RandomElement<T>(this IList<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (collection.Count == 0)
                throw new InvalidOperationException("Sequence contains no elements");

            return collection[random.Value.Next(collection.Count)];
        }
    }
}
