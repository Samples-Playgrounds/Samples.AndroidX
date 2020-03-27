using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Toggl.Shared.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> self, IEnumerable<T> items)
        {
            Ensure.Argument.IsNotNull(self, nameof(self));
            Ensure.Argument.IsNotNull(items, nameof(items));

            foreach (var item in items)
                self.Add(item);
        }
    }
}
