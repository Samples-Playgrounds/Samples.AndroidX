using System;
using System.Collections.Generic;

namespace Toggl.Core.UI.Collections
{
    [Obsolete("We are moving into using CollectionSection and per platform diffing")]
    public interface IGroupOrderedCollection<TItem> : IReadOnlyList<IReadOnlyList<TItem>>
    {
        bool IsEmpty { get; }

        SectionedIndex? IndexOf(TItem item);
        SectionedIndex? IndexOf(IComparable itemId);

        (SectionedIndex index, bool needsNewSection) InsertItem(TItem item);
        SectionedIndex? UpdateItem(IComparable key, TItem item);
        void ReplaceWith(IEnumerable<TItem> items);
        TItem RemoveItemAt(int section, int row);
    }
}
