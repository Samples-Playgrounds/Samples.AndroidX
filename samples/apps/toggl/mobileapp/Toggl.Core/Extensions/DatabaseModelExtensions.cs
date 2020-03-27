using System.Linq;
using Toggl.Shared.Models;
using Toggl.Storage.Models;

namespace Toggl.Core.Extensions
{
    public static class DatabaseModelExtensions
    {
        public static bool DiffersFrom<T>(this T first, T second)
            where T : IDatabaseModel
            => first is ILastChangedDatable currentLastChangedDatable &&
               second is ILastChangedDatable originalLastChangedDatable
                ? currentLastChangedDatable.At != originalLastChangedDatable.At
                : anyPropertyChanged(first, second);

        private static bool anyPropertyChanged<TDatabase>(TDatabase first, TDatabase second)
            => typeof(TDatabase).GetProperties().Any(
                property => !property.GetValue(first, null).Equals(property.GetValue(second, null)));
    }
}
