using Microsoft.Reactive.Testing;
using System;
using System.Reactive;
using System.Reactive.Disposables;

namespace Toggl.Core.Tests.TestExtensions
{
    public static class ReactiveExtensions
    {
        public static void Schedule(this TestScheduler self, TimeSpan ticks, Action action)
            => self.Schedule(Unit.Default, ticks, (_, __) =>
            {
                action();
                return Disposable.Empty;
            });
    }
}