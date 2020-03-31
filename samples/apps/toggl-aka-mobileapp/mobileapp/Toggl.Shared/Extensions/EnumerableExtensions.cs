﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl.Shared.Extensions
{
    public static class EnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> self, Func<T, bool> predicate)
        {
            int i = 0;
            foreach (var item in self)
            {
                if (predicate(item))
                    return i;
                i++;
            }
            return -1;
        }

        public static int IndexOf<T>(this IEnumerable<T> self, T item, Func<T, IComparable> indexKey)
        {
            var itemKey = indexKey(item);
            return self.IndexOf(i => indexKey(i).CompareTo(itemKey) == 0);
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> self, Func<T, IComparable> orderingKey, bool isDescending)
        {
            return isDescending
                ? self.OrderByDescending(orderingKey)
                : self.OrderBy(orderingKey);
        }

        public static int GroupIndexOf<T>(this IEnumerable<IEnumerable<T>> self, T item, Func<T, IComparable> groupingKey)
        {
            var groupKey = groupingKey(item);
            return self.IndexOf(g => groupingKey(g.First()).CompareTo(groupKey) == 0);
        }

        public static IEnumerable<TRight> SelectAllRight<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> self)
            => self.Where(either => either.IsRight).Select(either => either.Right);

        public static IEnumerable<TLeft> SelectAllLeft<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> self)
            => self.Where(either => either.IsLeft).Select(either => either.Left);

        public static IEnumerable<T> SelectNonNulls<T>(this IEnumerable<T?> self) where T : struct
            => self.Where(nullable => nullable.HasValue).Select(nullable => nullable.Value);

        public static bool None<T>(this IEnumerable<T> collection, Func<T, bool> condition)
            => !collection.Any(condition);

        public static bool None<T>(this IEnumerable<T> collection)
           => !collection.Any();

        public static TItem MaxBy<TItem, TProperty>(this IEnumerable<TItem> collection, Func<TItem, TProperty> keySelector)
            where TProperty : IComparable<TProperty>
        {
            Ensure.Argument.IsNotNull(collection, nameof(collection));

            if (collection.None())
                throw new InvalidOperationException("The collection is empty");

            var maxItem = collection.First();
            var maxKey = keySelector(maxItem);

            foreach (var item in collection)
            {
                var key = keySelector(item);
                if (key.CompareTo(maxKey) > 0)
                {
                    maxKey = key;
                    maxItem = item;
                }
            }

            return maxItem;
        }

        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        public static TimeSpan Sum<T>(this IEnumerable<T> collection, Func<T, TimeSpan> selectTimeSpan)
            => collection.Aggregate(TimeSpan.Zero, (acc, item) => acc + selectTimeSpan(item));

        public static TimeSpan Sum<T>(this IEnumerable<T> collection, Func<T, TimeSpan?> selectTimespan)
            => collection.Sum(item => selectTimespan(item) ?? TimeSpan.Zero);

        public static bool ContainsExactlyAll<T>(this IReadOnlyList<T> collection, IReadOnlyList<T> otherCollection)
            => collection.Count == otherCollection.Count
               && collection.Except(otherCollection).None();

        public static bool SetEquals<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer = null)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));

            comparer = comparer ?? EqualityComparer<T>.Default;

            var hashSet = new HashSet<T>(first, comparer);

            return hashSet.SetEquals(second);
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> observable)
            => observable.SelectMany(CommonFunctions.Identity);

        public static IEnumerable<T> PrependIf<T>(this IEnumerable<T> collection, bool condition, Func<T> getElement)
            => condition ? collection.Prepend(getElement()) : collection;
    }
}
