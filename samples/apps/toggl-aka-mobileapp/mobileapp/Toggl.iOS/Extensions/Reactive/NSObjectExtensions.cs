using Foundation;
using Toggl.Core.UI.Reactive;

namespace Toggl.iOS.Extensions
{
    public static class NSObjectExtensions
    {
        public static IReactive<T> Rx<T>(this T type) where T : NSObject
            => new ReactiveNSObject<T>(type);

        private class ReactiveNSObject<T> : IReactive<T>
            where T : NSObject
        {
            public T Base { get; }

            public ReactiveNSObject(T @base)
            {
                Base = @base;
            }
        }
    }
}
