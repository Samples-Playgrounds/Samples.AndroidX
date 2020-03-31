using System.Collections.Generic;
using System.Collections.Immutable;

namespace Toggl.Core.UI.Collections
{
    public class SectionModel<THeader, TItem> : ISectionModel<THeader, TItem>
    {
        public THeader Header { get; private set; }
        public IImmutableList<TItem> Items { get; private set; }

        public SectionModel()
        {
        }

        public SectionModel(THeader header, IEnumerable<TItem> items)
        {
            Header = header;
            Items = items.ToImmutableList();
        }

        public void Initialize(THeader header, IEnumerable<TItem> items)
        {
            Header = header;
            Items = items.ToImmutableList();
        }

        public static SectionModel<THeader, TItem> SingleElement(TItem item)
            => new SectionModel<THeader, TItem>(default(THeader), new[] { item });
    }
}
