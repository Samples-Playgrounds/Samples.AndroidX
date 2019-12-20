using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Reactive;
using Toggl.iOS.Views;

namespace Toggl.iOS.Extensions.Reactive
{
    public static class HueSaturationPickerViewExtensions
    {
        public static IObservable<float> Hue(this IReactive<HueSaturationPickerView> reactive)
            => Observable
                .FromEventPattern(e => reactive.Base.HueChanged += e, e => reactive.Base.HueChanged -= e)
                .Select(e => ((HueSaturationPickerView)e.Sender).Hue);

        public static IObservable<float> Saturation(this IReactive<HueSaturationPickerView> reactive)
            => Observable
                .FromEventPattern(e => reactive.Base.SaturationChanged += e, e => reactive.Base.SaturationChanged -= e)
                .Select(e => ((HueSaturationPickerView)e.Sender).Saturation);

        public static Action<float> HueObserver(this IReactive<HueSaturationPickerView> reactive)
            => hue => reactive.Base.Hue = hue;

        public static Action<float> SaturationObserver(this IReactive<HueSaturationPickerView> reactive)
            => saturation => reactive.Base.Saturation = saturation;

        public static Action<float> ValueObserver(this IReactive<HueSaturationPickerView> reactive)
            => value => reactive.Base.Value = value;
    }
}
