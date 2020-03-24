using System;

namespace Toggl.Droid.Adapters.DiffingStrategies
{
    public interface IDiffingStrategy<T> where T : IEquatable<T>
    {
        bool AreContentsTheSame(T item, T other);
        bool AreItemsTheSame(T item, T other);
        bool HasStableIds { get; }
        long GetItemId(T item);
    }
}
