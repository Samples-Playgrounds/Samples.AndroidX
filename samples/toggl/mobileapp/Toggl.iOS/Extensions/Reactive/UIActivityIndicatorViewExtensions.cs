using CoreGraphics;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Reactive;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Extensions.Reactive
{
    public static class UIActivityIndicatorViewExtensions
    {
        public static Action<bool> IsAnimating(this IReactive<UIActivityIndicatorView> reactive)
            => isVisible =>
            {
                if (isVisible)
                    reactive.Base.StartAnimating();
                else
                    reactive.Base.StopAnimating();
            };
    }
}
