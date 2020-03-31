using System;

namespace Toggl.Core.UI.Collections
{
    [Obsolete("We are moving into using CollectionSection and per platform diffing")]
    public struct SectionedIndex
    {
        public int Section { get; }
        public int Row { get; }

        public SectionedIndex(int section, int row)
        {
            Section = section;
            Row = row;
        }

        public override string ToString()
        {
            return $"{Section}-{Row}";
        }
    }
}
