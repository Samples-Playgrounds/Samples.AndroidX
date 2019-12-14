using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Reactive;
using UIKit;

namespace Toggl.iOS.Extensions.Reactive
{
    public static class UIDatePickerExtensions
    {
        public static IObservable<DateTimeOffset> Date(this IReactive<UIDatePicker> reactive)
            => Observable
                .FromEventPattern(e => reactive.Base.ValueChanged += e, e => reactive.Base.ValueChanged -= e)
                .Select(e => ((UIDatePicker)e.Sender).Date.ToDateTimeOffset());

        public static IObservable<DateTimeOffset> DateComponent(this IReactive<UIDatePicker> reactive)
            => reactive.Date()
                .StartWith(reactive.Base.Date.ToDateTimeOffset())
                .DistinctUntilChanged(d => d.Date)
                .Skip(1);

        public static IObservable<DateTimeOffset> TimeComponent(this IReactive<UIDatePicker> reactive)
            => reactive.Date()
                .StartWith(reactive.Base.Date.ToDateTimeOffset())
                .DistinctUntilChanged(d => d.TimeOfDay)
                .Skip(1);

        public static Action<DateTimeOffset> DateTimeObserver(this IReactive<UIDatePicker> reactive)
            => dateTime => reactive.Base.SetDate(dateTime.ToNSDate(), true);
    }
}
