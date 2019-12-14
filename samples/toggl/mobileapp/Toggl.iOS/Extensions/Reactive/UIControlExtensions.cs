using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toggl.Core.UI.Reactive;
using UIKit;

namespace Toggl.iOS.Extensions.Reactive
{
    public static class UIControlExtensions
    {
        public static Action<bool> Enabled(this IReactive<UIControl> reactive)
            => enabled => reactive.Base.Enabled = enabled;

        public static IObservable<Unit> Changed(this IReactive<UIControl> reactive)
            => Observable.Create<Unit>(observer =>
            {
                void changed(object sender, EventArgs args)
                {
                    observer.OnNext(Unit.Default);
                }

                reactive.Base.ValueChanged += changed;

                return Disposable.Create(() => reactive.Base.ValueChanged -= changed);
            });
    }
}
