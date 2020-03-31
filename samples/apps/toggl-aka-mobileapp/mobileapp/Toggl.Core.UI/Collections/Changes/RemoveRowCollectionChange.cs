using System;

namespace Toggl.Core.UI.Collections.Changes
{
    [Obsolete("We are moving into using CollectionSection and per platform diffing")]
    public struct RemoveRowCollectionChange : ICollectionChange
    {
        public SectionedIndex Index { get; }

        public RemoveRowCollectionChange(SectionedIndex index)
        {
            Index = index;
        }

        public override string ToString() => $"Remove row: {Index}";
    }
}
