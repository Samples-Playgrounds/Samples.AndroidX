using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Reactive;
using Toggl.iOS.Views.EditDuration;

namespace Toggl.iOS.Extensions.Reactive
{
    public static class WheelForegroundViewExtensions
    {
        public static IObservable<DateTimeOffset> StartTime(this IReactive<WheelForegroundView> reactive)
            => Observable
                .FromEventPattern(e => reactive.Base.StartTimeChanged += e, e => reactive.Base.StartTimeChanged -= e)
                .Select(_ => reactive.Base.StartTime);

        public static IObservable<DateTimeOffset> EndTime(this IReactive<WheelForegroundView> reactive)
            => Observable
                .FromEventPattern(e => reactive.Base.EndTimeChanged += e, e => reactive.Base.EndTimeChanged -= e)
                .Select(_ => reactive.Base.EndTime);
    }
}
