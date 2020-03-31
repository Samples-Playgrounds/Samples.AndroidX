using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Toggl.Core.UI.Collections
{
    public class AnimatableSectionModel<THeader, TItem, TKey> : IAnimatableSectionModel<THeader, TItem, TKey>
        where TKey : IEquatable<TKey>
        where THeader : IDiffable<TKey>
        where TItem : IDiffable<TKey>, IEquatable<TItem>
    {
        public THeader Header { get; private set; }
        public IImmutableList<TItem> Items { get; private set; }

        public TKey Identity { get; private set; }

        public AnimatableSectionModel()
        {

        }

        public AnimatableSectionModel(THeader header, IEnumerable<TItem> items)
        {
            Header = header;
            Items = items.ToImmutableList();
            Identity = Header.Identity;
        }

        public void Initialize(THeader header, IEnumerable<TItem> items)
        {
            Header = header;
            Items = items.ToImmutableList();
            Identity = Header.Identity;
        }
    }
}
