using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Reactive;
using Toggl.Droid.Views;
using static Android.Widget.SeekBar;

namespace Toggl.Droid.Extensions.Reactive
{
    public static class ValueSliderViewExtensions
    {
        public static IObservable<int> Progress(this IReactive<ValueSlider> reactive)
            => Observable
                .FromEventPattern<ProgressChangedEventArgs>(e => reactive.Base.ProgressChanged += e, e => reactive.Base.ProgressChanged -= e)
                .Select(args => ((ValueSlider)args.Sender).Progress);

        public static Action<int> ProgressObservable(this IReactive<ValueSlider> reactive)
            => progress => reactive.Base.Progress = progress;

        public static Action<float> HueObserver(this IReactive<ValueSlider> reactive)
          => hue => reactive.Base.Hue = hue;

        public static Action<float> SaturationObserver(this IReactive<ValueSlider> reactive)
            => saturation => reactive.Base.Saturation = saturation;
    }
}
