using Java.Lang;

namespace Toggl.Droid.Extensions
{
    public static class JavaUtils
    {
        public static Class ToClass<T>()
            => Class.FromType(typeof(T));
    }
}
