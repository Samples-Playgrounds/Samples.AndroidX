using System;
using Toggl.Core.UI.Reactive;
using Toggl.iOS.Views;

namespace Toggl.iOS.Extensions.Reactive
{
    public static class FadeViewExtensions
    {
        public static Action<bool> FadeRight(this IReactive<FadeView> reactive)
            => useRightFading => reactive.Base.FadeRight = useRightFading;

        public static Action<bool> FadeLeft(this IReactive<FadeView> reactive)
            => useLeftFading => reactive.Base.FadeLeft = useLeftFading;
    }
}
