using Java.Lang;
using Toggl.Core.UI.Reactive;

namespace Toggl.Droid.Extensions.Reactive
{
    public static class JavaObjectExtensions
    {
        public static IReactive<T> Rx<T>(this T type) where T : Object
            => new ReactiveJavaObject<T>(type);

        private class ReactiveJavaObject<T> : IReactive<T>
            where T : Object
        {
            public T Base { get; }

            public ReactiveJavaObject(T @base)
            {
                Base = @base;
            }
        }
    }
}
