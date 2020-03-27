using System;

namespace Toggl.Core.UI.Collections.Diffing
{
    public sealed class ItemPath : IEquatable<ItemPath>
    {
        public int sectionIndex { get; }
        public int itemIndex { get; }

        public ItemPath(int sectionIndex, int itemIndex)
        {
            this.sectionIndex = sectionIndex;
            this.itemIndex = itemIndex;
        }

        public bool Equals(ItemPath other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return sectionIndex == other.sectionIndex && itemIndex == other.itemIndex;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ItemPath)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (sectionIndex * 397) ^ itemIndex;
            }
        }
    }
}
