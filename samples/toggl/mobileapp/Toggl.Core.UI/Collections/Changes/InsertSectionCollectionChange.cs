using System;

namespace Toggl.Core.UI.Collections.Changes
{
    [Obsolete("We are moving into using CollectionSection and per platform diffing")]
    public struct InsertSectionCollectionChange<T> : ICollectionChange
    {
        public int Index { get; }

        public T Item { get; }

        public InsertSectionCollectionChange(int index, T item)
        {
            Index = index;
            Item = item;
        }

        public override string ToString() => $"Insert Section: {Index} ({Item})";
    }
}
