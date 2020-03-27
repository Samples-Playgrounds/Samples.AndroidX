using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Reactive;
using Toggl.iOS.Views.EditDuration;

namespace Toggl.iOS.Extensions.Reactive
{
    public static class DurationFieldExtensions
    {
        public static IObservable<TimeSpan> Duration(this IReactive<DurationField> reactive)
            => Observable
                .FromEventPattern(handler => reactive.Base.DurationChanged += handler, handler => reactive.Base.DurationChanged -= handler)
                .Select(_ => reactive.Base.Duration);
    }
}
