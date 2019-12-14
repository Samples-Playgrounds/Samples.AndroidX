using Microsoft.Reactive.Testing;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;

namespace Toggl.Core.Tests.TestExtensions
{
    public static class ITestableObserverExtensions
    {
        public static T SingleEmittedValue<T>(this ITestableObserver<T> observer)
            => observer.Values().Single();

        public static T LastEmittedValue<T>(this ITestableObserver<T> observer)
            => observer.Values().Last();

        public static IEnumerable<T> Values<T>(this ITestableObserver<T> observer)
            => observer.Messages
                .Select(recorded => recorded.Value)
                .Where(notification => notification.Kind == NotificationKind.OnNext)
                .Select(notification => notification.Value);
    }
}
