using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Reactive;
using UIKit;

namespace Toggl.iOS.Extensions.Reactive
{
    public static class UISliderExtensions
    {
        public static IObservable<float> Value(this IReactive<UISlider> reactive)
            => Observable
                .FromEventPattern(e => reactive.Base.ValueChanged += e, e => reactive.Base.ValueChanged -= e)
                .Select(e => ((UISlider)e.Sender).Value);
    }
}
