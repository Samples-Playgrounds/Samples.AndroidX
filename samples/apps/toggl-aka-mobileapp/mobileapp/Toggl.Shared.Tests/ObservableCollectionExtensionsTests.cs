using FluentAssertions;
using System.Collections.ObjectModel;
using Xunit;
using static Toggl.Shared.Extensions.ObservableCollectionExtensions;

namespace Toggl.Shared.Tests
{
    public sealed class ObservableCollectionExtensionsTests
    {
        public sealed class TheAddRangeMethod
        {
            [Fact, LogIfTooSlow]
            public void AddsAllItems()
            {
                int[] initialItems = { 1, 2 };
                int[] newItems = { 3, 4, 5 };
                var collection = new ObservableCollection<int>(initialItems);

                collection.AddRange(newItems);

                collection.Should().HaveCount(initialItems.Length + newItems.Length)
                          .And.Contain(initialItems)
                          .And.Contain(newItems);
            }
        }
    }
}
