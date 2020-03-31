using System;

namespace Toggl.Core.UI.Collections.Changes
{
    [Obsolete("We are moving into using CollectionSection and per platform diffing")]
    public struct UpdateRowCollectionChange<T> : ICollectionChange
    {
        public SectionedIndex Index { get; }

        public T Item { get; }

        public UpdateRowCollectionChange(SectionedIndex index, T item)
        {
            Index = index;
            Item = item;
        }

        public override string ToString() => $"Update row: {Index} ({Item})";
    }
}
