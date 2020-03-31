using System;

namespace Toggl.Core.UI.Collections.Changes
{
    [Obsolete("We are moving into using CollectionSection and per platform diffing")]
    public struct MoveRowToNewSectionCollectionChange<T> : ICollectionChange
    {
        public SectionedIndex OldIndex { get; }

        public int Index { get; }

        public T Item { get; }

        public MoveRowToNewSectionCollectionChange(SectionedIndex oldIndex, int index, T item)
        {
            OldIndex = oldIndex;
            Index = index;
            Item = item;
        }

        public override string ToString() => $"Move to new section: {OldIndex} -> {Index} ({Item})";
    }
}
