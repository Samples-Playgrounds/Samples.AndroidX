using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Toggl.Core.UI.Collections
{
    public interface IDiffable<TKey>
        where TKey : IEquatable<TKey>
    {
        TKey Identity { get; }
    }

    public interface ISectionModel<THeader, TItem>
    {
        THeader Header { get; }
        IImmutableList<TItem> Items { get; }
        void Initialize(THeader header, IEnumerable<TItem> items);
    }

    public interface IAnimatableSectionModel<TSection, TItem, TKey> : ISectionModel<TSection, TItem>, IDiffable<TKey>
        where TKey : IEquatable<TKey>
        where TItem : IDiffable<TKey>, IEquatable<TItem>
    { }
}
