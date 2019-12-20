using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.UI.Reactive;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Extensions
{
    public static class UIBarButtonItemExtensions
    {
        public static Action<bool> Enabled(this IReactive<UIBarButtonItem> reactive)
            => enabled => reactive.Base.Enabled = enabled;

        public static IObservable<Unit> Tap(this IReactive<UIBarButtonItem> reactive)
            => Observable
                .FromEventPattern(e => reactive.Base.Clicked += e, e => reactive.Base.Clicked -= e)
                .SelectUnit();
    }
}
