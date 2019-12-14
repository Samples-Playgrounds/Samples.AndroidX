namespace Toggl.Core.Extensions
{
    public static class TypeExtensions
    {
        public static string GetSafeTypeName<T>(this T item)
            => item?.GetType().Name ?? "[null]";
    }
}
