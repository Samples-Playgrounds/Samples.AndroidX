using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Reactive;
using Toggl.Droid.Views;

namespace Toggl.Droid.Extensions.Reactive
{
    public static class HueSaturationPickerViewExtensions
    {
        public static IObservable<float> Hue(this IReactive<HueSaturationPickerView> reactive)
            => Observable
                .FromEventPattern(e => reactive.Base.HueChanged += e, e => reactive.Base.HueChanged -= e)
                .Select(args => ((HueSaturationPickerView)args.Sender).Hue);

        public static IObservable<float> Saturation(this IReactive<HueSaturationPickerView> reactive)
            => Observable
                .FromEventPattern(e => reactive.Base.SaturationChanged += e, e => reactive.Base.SaturationChanged -= e)
                .Select(args => ((HueSaturationPickerView)args.Sender).Saturation);

        public static Action<float> HueObserver(this IReactive<HueSaturationPickerView> reactive)
            => hue => reactive.Base.Hue = hue;

        public static Action<float> SaturationObserver(this IReactive<HueSaturationPickerView> reactive)
            => saturation => reactive.Base.Saturation = saturation;

        public static Action<float> ValueObserver(this IReactive<HueSaturationPickerView> reactive)
            => value => reactive.Base.Value = value;
    }
}
